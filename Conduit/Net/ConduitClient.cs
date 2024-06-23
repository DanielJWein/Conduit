using System.Linq;

using Conduit.Codec;
using Conduit.Net.Events;
using Conduit.Net.Exceptions;
using Conduit.Util;

namespace Conduit.Net;

/// <summary>
/// Gets Conduit encoded data from a Conduit ConduitServer.
/// </summary>
public class ConduitClient : IDisposable {

    /// <summary>
    /// Holds the current track title and raises events when it is changed
    /// </summary>
    public Alerter<string> CurrentTrackTitle = new( "(nothing)" );

    /// <summary>
    /// Holds the Maximum Frame Size
    /// </summary>
    protected const int MAX_FRAME_SIZE = 8192;

    protected ConduitConnection? serverConnection;

    /// <summary>
    /// Holds a local copy of the data
    /// </summary>
    private readonly ByteQueue data;

    private bool connected = false;

    private bool disposedValue;

    /// <summary>
    /// Creates a ConduitTurnkeyClient with a specified address
    /// </summary>
    /// <param name="address"> The address to connect to </param>
    /// <param name="port">    The port to connect to </param>
    public ConduitClient( string address, ushort port = 32662 ) {
        ServerEndpoint =
            new IPEndPoint(
                address.Any( char.IsLetter ) ?
                    Dns.GetHostEntry( address ).AddressList[ 0 ]
                    : IPAddress.Parse( address ),
                port
            );

        data = new( MAX_FRAME_SIZE );
        CurrentTrackTitle.ValueChanged += ( object s, EventArgs e ) => OnTrackTitleChanged?.Invoke( s, e );
    }

    /// <summary>
    /// Destroys this object
    /// </summary>
    ~ConduitClient( )
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        => Dispose( disposing: false );

    /// <summary>
    /// Gets whether or not the client is connected
    /// </summary>
    public bool Connected {
        private set {
            if ( connected != value ) {
                if ( value ) {
                    OnConnected?.Invoke( this, EventArgs.Empty );
                }
                else {
                    OnDisconnected?.Invoke( this, EventArgs.Empty );
                }
                connected = value;
            }
        }
        get => connected;
    }

    /// <summary>
    /// Holds the IPEndPoint of the server.
    /// </summary>
    protected IPEndPoint ServerEndpoint { set; get; }

    /// <summary>
    /// Holds the socket
    /// </summary>
    protected Socket Socket => serverConnection?.GetSocket( );

    /// <summary>
    /// Run when this client connects to a server
    /// </summary>
    public event EventHandler OnConnected;

    /// <summary>
    /// Invoked when a control packet is received.
    /// </summary>
    public event EventHandler<ControlEventArgs> OnControlPacketReceived;

    /// <summary>
    /// Runs when this client disconnects from a server
    /// </summary>
    public event EventHandler OnDisconnected;

    /// <summary>
    /// Runs when the track title changes
    /// </summary>
    public event EventHandler OnTrackTitleChanged;

    /// <summary>
    /// Connects to the Conduit Server.
    /// </summary>
    public void Connect( ) {
        Socket socket = new  ( ServerEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp ) {
            ReceiveBufferSize = 256 * 1024 //256 Kilobytes
        };
        socket.Connect( ServerEndpoint );
        serverConnection = new( socket, ServerEndpoint );

        Connected = true;
    }

    /// <summary>
    /// Disconnects from the server.
    /// </summary>
    public void Disconnect( ) {
        serverConnection?.Close( );
        Connected = false;
    }

    /// <summary>
    /// Destroys this object
    /// </summary>
    public void Dispose( ) {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose( disposing: true );
        GC.SuppressFinalize( this );
    }

    /// <summary>
    /// Gets a frame from the Conduit Server
    /// </summary>
    /// <returns> A ConduitCodecFrame with the encoded Opus data. </returns>
    public ConduitCodecFrame GetFrame( ) {
        if ( !Connected ) {
            return null;
        }

        lock ( data ) {
            data?.Clear( );
            byte[ ] bufma = new byte[2];
            //Eat the packet header
            serverConnection.Receive( bufma, 0, 2 );
            //Get the packet length
            serverConnection.Receive( bufma, 0, 2 );

            ushort packetLength = BitConverter.ToUInt16( bufma, 0 );

            //Quick sanity check to blast it should we desync somehow.
            if ( packetLength > MAX_FRAME_SIZE ) {
                throw new ConduitException( "The socket data desynchronized." );
            }

            data?.Clear( );
            //Receive the full packet
            serverConnection.Receive( data, 0, packetLength );

            return new( data, packetLength );
        }
    }

    /// <summary>
    /// Processes control packets
    /// </summary>
    /// <returns> True, if a control packet was processed </returns>
    /// <exception cref="DisconnectedException"> Thrown if the control packet was <see cref="ConduitControlPacket.CONTROL_DISCONNECT" /> </exception>
    public bool ProcessControlPacket( ) {
        byte[] bufma = new byte[2];

        bufma = serverConnection.Receive( 2, true );

        //Process control packets
        return processControlPacket( bufma );
    }

    /// <summary>
    /// Clears all data in this server
    /// </summary>
    protected virtual void ClearData( ) => data.Clear( );

    /// <summary>
    /// Destroys this object
    /// </summary>
    /// <param name="disposing"> Free managed resources as well </param>
    protected virtual void Dispose( bool disposing ) {
        if ( !disposedValue ) {
            Disconnect( );
            Socket?.Dispose( );

            disposedValue = true;
        }
    }

    /// <summary>
    /// Processes a Control Packet
    /// </summary>
    /// <returns> True, if the packet was a control packet. </returns>
    private bool processControlPacket( byte[ ] data ) {
        if ( !Connected ) {
            return false;
        }

        byte [] control = new byte[2];
        Array.Copy( data, control, 2 );
        //This is a control packet!
        if ( data[ 0 ] == 0x01 ) {
            if ( control.CheckAgainst( ConduitControlPacket.CONTROL_DATA ) )
                //This is a data packet after all.
                return false;

            //If it's not data, it's a control!
            OnControlPacketReceived?.Invoke( this, new( data ) );

            //Message Disconnect
            if ( control.CheckAgainst( ConduitControlPacket.CONTROL_DISCONNECT ) ) {
                Disconnect( );
                OnDisconnected?.Invoke( this, null );
                throw new DisconnectedException( );
            }

            //Track Changed
            if ( control.CheckAgainst( ConduitControlPacket.CONTROL_TRACK_TITLE_CHANGED ) ) {
                //Read the control message fully.
                serverConnection.Receive( control, 0, 2 );
                try {
                    CurrentTrackTitle.Value = serverConnection.GetString( );
                }
                catch ( SocketException ) {
                    Disconnect( );
                }
                return true;
            }

            //Eat the message.
            serverConnection.Receive( control, 0, 2 );
            //We aren't going to return a packet.
            return true;
        }
        return false;
    }
}
