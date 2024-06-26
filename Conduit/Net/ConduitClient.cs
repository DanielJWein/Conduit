﻿using System.Linq;

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
    /// Holds the status information for this client
    /// </summary>
    public ConduitClientStatus Status;

    /// <summary>
    /// Holds the Maximum Frame Size
    /// </summary>
    protected const int MAX_FRAME_SIZE = 8192;

    /// <summary>
    /// True if this client has been disposed.
    /// </summary>
    protected bool disposed;

    /// <summary>
    /// Holds this client's connection to a server
    /// </summary>
    protected ConduitConnection? serverConnection;

    /// <summary>
    /// Holds a local copy of the data
    /// </summary>
    private readonly ByteQueue data;

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

        Status = new( );
        Status.CurrentTrackTitle.ValueChanged += ( object s, EventArgs e ) => OnTrackTitleChanged?.Invoke( s, e );
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
    /// <exception cref="ObjectDisposedException"> Thrown if this client is disposed. </exception>
    public void Connect( ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        Socket socket = new  ( ServerEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp ) {
            ReceiveBufferSize = 256 * 1024 //256 Kilobytes
        };
        socket.Connect( ServerEndpoint );
        serverConnection = new( socket, ServerEndpoint );
        if ( !Status.Connected ) {
            Status.Connected = true;
            OnConnected?.Invoke( this, EventArgs.Empty );
        }
        serverConnection.SendControlPacket( ConduitControlPacket.CONTROL_CLIENT_REQUEST_TRACK_TITLE );
        Update( );
        SetReady( true );
    }

    /// <summary>
    /// Disconnects from the server.
    /// </summary>
    /// <exception cref="ObjectDisposedException"> Thrown if this client is disposed. </exception>
    public void Disconnect( ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        serverConnection?.Close( );
        if ( Status.Connected ) {
            Status.Connected = false;
            OnDisconnected?.Invoke( this, EventArgs.Empty );
        }
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
    /// <exception cref="ObjectDisposedException"> Thrown if this client is disposed. </exception>
    /// <exception cref="ConduitException">
    /// Thrown if the socket data desynchronizes (see remarks).
    /// </exception>
    /// <remarks>
    /// A <see cref="ConduitException" /> is thrown if the packet size of a received packet exceeds <see cref="MAX_FRAME_SIZE" />.
    ///
    /// This is usually an indication of a desynchronization, and could lead to some... choice...
    /// audio being played in bad circumstances.
    ///
    /// In order to not destroy your ears, we give up instead.
    /// </remarks>
    public ConduitCodecFrame GetFrame( ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        if ( !Status.Connected ) {
            return null;
        }

        lock ( data ) {
            data?.Clear( );
            byte[ ] bufma = new byte[2];
            //Eat the packet header
            serverConnection.Receive( bufma, 0, 2 );

            if ( bufma[ 0 ] != 1 ) {
                throw new ConduitPacketException( "The header version was not 1!" );
            }
            if ( bufma[ 1 ] != 0 ) {
                //This is a control packet.
                processControlPacket( bufma );
            }

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
    /// <exception cref="ObjectDisposedException"> Thrown if this client is disposed. </exception>
    public bool Update( ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        return processControlPacket( serverConnection.Receive( 2, true ) );
    }

    /// <summary>
    /// Clears all data in this server
    /// </summary>
    /// <exception cref="ObjectDisposedException"> Thrown if this client is disposed. </exception>
    protected virtual void ClearData( ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        data.Clear( );
    }

    /// <summary>
    /// Destroys this object
    /// </summary>
    /// <param name="disposing"> Free managed resources as well </param>
    protected virtual void Dispose( bool disposing ) {
        if ( !disposed ) {
            Disconnect( );
            Socket?.Dispose( );

            disposed = true;
        }
    }

    protected void SetReady( bool ready ) {
        if ( serverConnection.Ready != ready ) {
            if ( ready ) {
                //Send that we are ready.
                serverConnection.SendControlPacket( ConduitControlPacket.CONTROL_CLIENT_READY );
            }
            else {
                serverConnection.SendControlPacket( ConduitControlPacket.CONTROL_CLIENT_NOT_READY );
            }
        }

        serverConnection.Ready = ready;
    }

    /// <summary>
    /// Processes a Control Packet
    /// </summary>
    /// <returns> True, if the packet was a control packet. </returns>
    /// <exception cref="ObjectDisposedException"> Thrown if this client is disposed. </exception>
    private bool processControlPacket( byte[ ] data ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        if ( !Status.Connected ) {
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
                    Status.CurrentTrackTitle.Value = serverConnection.GetString( );
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
