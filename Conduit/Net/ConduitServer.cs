using System.Collections.Generic;
using System.Threading.Tasks;

using Conduit.Codec;
using Conduit.Net.Events;
using Conduit.Util;

namespace Conduit.Net;

/// <summary>
/// Sends conduit-encoded data to connected clients
/// </summary>
public class ConduitServer : IDisposable {

    /// <summary>
    /// If true, the server will send empty packets.
    /// </summary>
    public bool SendEmptyPackets = false;

    /// <summary>
    /// The track title that is currently playing
    /// </summary>
    public Alerter<string> TrackTitle = new( );

    /// <summary>
    /// Holds the server's socket
    /// </summary>
    protected readonly Socket serverSocket;

    private readonly List<ConduitConnection> clientele = new( 16 );

    private bool disposedValue;

    private ServerListener listener;

    /// <summary>
    /// Creates a new ConduitServer
    /// </summary>
    public ConduitServer( IPAddress bindAddress = null, ushort bindPort = 32662 ) {
        serverSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
        serverSocket.Bind( new IPEndPoint( bindAddress ?? IPAddress.Any, bindPort ) );
        serverSocket.SendBufferSize = 8192;

        listener = new( serverSocket );

        //Alias the ClientConnected event
        listener.ClientConnected += ( object? s, ClientConnectedEventArgs args ) => {
            clientele.Add( new ConduitConnection( args.Socket, args.Address ) );
            OnClientConnected?.Invoke( s, args );
        };

        TrackTitle.ValueChanged += ( object s, EventArgs e ) => Send( ConduitControlPacket.CONTROL_TRACK_TITLE_CHANGED );
    }

    /// <summary>
    /// Destroys this object.
    /// </summary>
    ~ConduitServer( ) {
        Dispose( disposing: false );
    }

    /// <summary>
    /// Holds a list of all connected clients
    /// </summary>
    public IReadOnlyList<ConduitConnection> Clients => clientele;

    /// <summary>
    /// Holds the amount of data the server has multicasted (before duplication to clients)
    /// </summary>
    public ulong DataCounter { get; protected set; } = 0;

    /// <summary>
    /// The approximate bitrate of the server's stream
    /// </summary>
    public double ExpectedBitrate { get; protected set; } = 1;

    /// <summary>
    /// Holds the amount of data the server has multicasted (after duplication to clients)
    /// </summary>
    public ulong TotalDataCounter { get; protected set; } = 0;

    /// <summary>
    /// Raised when a client connects
    /// </summary>
    public event EventHandler<ClientConnectedEventArgs> OnClientConnected;

    /// <summary>
    /// Raised when a client disconnects
    /// </summary>
    public event EventHandler<ClientDisconnectedEventArgs> OnClientDisconnected;

    /// <summary>
    /// Raised when KillAllConnections is executed
    /// </summary>
    public event EventHandler OnDisconnectAllClients;

    /// <summary>
    /// Raised when data is sent
    /// </summary>
    public event EventHandler OnEmitData;

    /// <summary>
    /// Run when the track title is changed.
    /// </summary>
    public event EventHandler OnTrackTitleChanged;

    /// <summary>
    /// Disconnects all connected clients.
    /// </summary>
    public void Close( ) {
        lock ( clientele ) {
            foreach ( ConduitConnection c in clientele ) {
                c.Close( );
                OnClientDisconnected?.Invoke( this, new( c.GetAddress( ) ) );
            }
            clientele.Clear( );
        }
        OnDisconnectAllClients?.Invoke( this, EventArgs.Empty );
    }

    /// <summary>
    /// Destroys this object.
    /// </summary>
    public void Dispose( ) {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose( disposing: true );
        GC.SuppressFinalize( this );
    }

    /// <summary>
    /// Disconnects a specified client
    /// </summary>
    public void Kill( ConduitConnection who ) {
        lock ( clientele ) {
            who.Close( );
            OnClientDisconnected?.Invoke( this, new( who.GetAddress( ) ) );
            clientele.RemoveAll( x => x.Closed );
        }
    }

    /// <summary>
    /// Sends data to all connected clients
    /// </summary>
    /// <param name="data"> The data to send </param>
    public void Send( ConduitCodecFrame data ) {
        if ( data.IsEmpty && !SendEmptyPackets )
            return;
        lock ( data ) {
            //Number of frames in a second
            double FramesPerSecond = 1000.0 / 60.0;
            //Get expected bitrate
            ExpectedBitrate = ( data.RealDataLength - 1 ) * FramesPerSecond * 8;

            //Lock clients to prevent new connections
            lock ( clientele ) {
                DataCounter += data.RealDataLength + 4u;
                clientele.RemoveAll( x => x.Closed );
                foreach ( var client in clientele ) {
                    try {
                        client.SendFrame( data );
                        TotalDataCounter += data.RealDataLength + 4u;
                        handleControlPacket( client );
                    }
                    catch {
                        client.Close( );
                    }
                }
                clientele.RemoveAll( x => x.Closed );
            }
        }
        OnEmitData?.Invoke( this, EventArgs.Empty );
    }

    /// <summary>
    /// Sends a control packet to the client
    /// </summary>
    /// <param name="ControlPacket"> The control packet to send </param>
    public void Send( byte[ ] ControlPacket ) {
        lock ( clientele ) {
            foreach ( ConduitConnection c in clientele ) {
                c.SendControlPacket( ControlPacket );
                //If we're sending the track title, send the track title.
                if ( ControlPacket.CheckAgainst( ConduitControlPacket.CONTROL_TRACK_TITLE_CHANGED ) ) {
                    c.SendString( TrackTitle );
                }
            }
        }
    }

    /// <summary>
    /// Starts listening for new connections
    /// </summary>
    public async Task StartListening( ) => await listener.StartListening( );

    /// <summary>
    /// Stops listening for new connections.
    /// </summary>
    public void StopListening( ) => listener.StopListening( );

    /// <summary>
    /// Destroys this object.
    /// </summary>
    /// <param name="disposing"> If true, clear managed resources as well </param>
    protected virtual void Dispose( bool disposing ) {
        if ( disposedValue )
            return;
        if ( disposing ) {
            Close( );
            clientele?.Clear( );
        }
        StopListening( );
        serverSocket?.Dispose( );
        listener.StopListening( );
        listener.Dispose( );
        disposedValue = true;
    }

    /// <summary>
    /// Handles control packets
    /// </summary>
    /// <param name="client"> The client to receive from </param>
    private void handleControlPacket( ConduitConnection client ) {
        Socket clientSocket = client.GetSocket();
        if ( clientSocket.Available > 1 ) {
            byte[] controlData = new byte[2];
            clientSocket.Receive( controlData, 2, SocketFlags.None );

            if ( controlData.CheckAgainst( ConduitControlPacket.CONTROL_DISCONNECT ) )
                client.Close( );
        }
    }
}
