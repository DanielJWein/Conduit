using System.Collections.Generic;
using System.Threading.Tasks;

using Conduit.Codec;
using Conduit.Net.Events;

namespace Conduit.Net;

/// <summary>
/// Sends conduit-encoded data to connected clients
/// </summary>
public class ConduitServer : IDisposable {

    /// <summary>
    /// Holds server configuration
    /// </summary>
    public ConduitServerOptions Configuration;

    /// <summary>
    /// Holds the server's status
    /// </summary>
    public ConduitServerStatus Status;

    /// <summary>
    /// Holds the server's socket
    /// </summary>
    protected readonly Socket serverSocket;

    /// <summary>
    /// True if the server has been disposed
    /// </summary>
    protected bool disposed;

    private readonly List<ConduitConnection> clients = new( 16 );

    private readonly ServerListener listener;

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
            clients.Add( new ConduitConnection( args.Socket, args.Address ) );
            OnClientConnected?.Invoke( s, args );
        };

        Configuration = new( );
        Status = new( );

        Status.TrackTitle.ValueChanged += ( object s, EventArgs e ) => Send( ConduitControlPacket.CONTROL_TRACK_TITLE_CHANGED );

        _ = listener.StartListening( );
    }

    /// <summary>
    /// Holds a list of all connected clients
    /// </summary>
    public IReadOnlyList<ConduitConnection> Clients => clients;

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
    /// <exception cref="ObjectDisposedException"> Thrown if this server is disposed. </exception>
    public void Close( ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        lock ( clients ) {
            foreach ( ConduitConnection c in clients ) {
                c.Close( );
                OnClientDisconnected?.Invoke( this, new( c.GetAddress( ) ) );
            }
            clients.Clear( );
        }
        OnDisconnectAllClients?.Invoke( this, EventArgs.Empty );
        listener.StopListening( );
    }

    /// <summary>
    /// Disconnects a specified client
    /// </summary>
    /// <exception cref="ObjectDisposedException"> Thrown if this server is disposed. </exception>
    public void DisconnectClient( ConduitConnection who ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        lock ( clients ) {
            who.Close( );
            OnClientDisconnected?.Invoke( this, new( who.GetAddress( ) ) );
            clients.RemoveAll( x => x.Closed );
        }
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
    /// Sends data to all connected clients
    /// </summary>
    /// <param name="data"> The data to send </param>
    /// <exception cref="ObjectDisposedException"> Thrown if this server is disposed. </exception>
    public void Send( ConduitCodecFrame data ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        if ( data.IsEmpty && !Configuration.SendEmptyPackets )
            return;
        lock ( data ) {
            //Number of frames in a second
            double FramesPerSecond = 1000.0 / 60.0;
            //Get expected bitrate
            Status.ExpectedBitrate = ( data.RealDataLength - 1 ) * FramesPerSecond * 8;

            //Lock clients to prevent new connections
            lock ( clients ) {
                Status.DataCounter += data.RealDataLength + 4u;
                UpdateClients( );
                clients.RemoveAll( x => x.Closed );
                for ( int i = 0; i < clients.Count; i++ ) {
                    ConduitConnection client = clients[ i ];
                    if ( !client.Ready ) {
                        continue;
                    }
                    try {
                        client.SendFrame( data );
                        Status.TotalDataCounter += data.RealDataLength + 4u;
                    }
                    catch {
                        client.Close( );
                    }
                }
                clients.RemoveAll( x => x.Closed );
            }
        }
        OnEmitData?.Invoke( this, EventArgs.Empty );
    }

    /// <summary>
    /// Sends a control packet to the client
    /// </summary>
    /// <param name="ControlPacket"> The control packet to send </param>
    /// <exception cref="ObjectDisposedException"> Thrown if this server is disposed. </exception>
    public void Send( byte[ ] ControlPacket ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        lock ( clients ) {
            foreach ( ConduitConnection c in clients ) {
                c.SendControlPacket( ControlPacket );
                //If we're sending the track title, send the track title.
                if ( ControlPacket.CheckAgainst( ConduitControlPacket.CONTROL_TRACK_TITLE_CHANGED ) ) {
                    c.SendString( Status.TrackTitle );
                }
            }
        }
    }

    /// <summary>
    /// Checks to see if clients have sent control packets.
    /// </summary>
    /// <exception cref="ObjectDisposedException"> Thrown if this server is disposed. </exception>
    public void UpdateClients( ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        for ( int i = 0; i < clients.Count; i++ ) {
            ConduitConnection client = clients[ i ];
            try {
                handleControlPacket( client );
            }
            catch {
                client.Close( );
            }
        }
    }

    /// <summary>
    /// Destroys this object.
    /// </summary>
    /// <param name="disposing"> If true, clear managed resources as well </param>
    protected virtual void Dispose( bool disposing ) {
        if ( disposed )
            return;
        if ( disposing ) {
            Close( );
            clients?.Clear( );
        }
        serverSocket?.Dispose( );
        listener.StopListening( );
        listener.Dispose( );
        disposed = true;
    }

    /// <summary>
    /// Handles control packets
    /// </summary>
    /// <param name="client"> The client to receive from </param>
    /// <exception cref="ObjectDisposedException"> Thrown if this server is disposed. </exception>
    private void handleControlPacket( ConduitConnection client ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        Socket clientSocket = client.GetSocket();
        if ( clientSocket.Available > 1 ) {
            byte[] controlData = new byte[2];
            clientSocket.Receive( controlData, 2, SocketFlags.None );

            if ( controlData.CheckAgainst( ConduitControlPacket.CONTROL_DISCONNECT ) ) {
                client.Close( );
                clients.Remove( client );
            }
            else if ( controlData.CheckAgainst( ConduitControlPacket.CONTROL_CLIENT_REQUEST_TRACK_TITLE ) ) {
                client.SendControlPacket( ConduitControlPacket.CONTROL_TRACK_TITLE_CHANGED );
                client.SendString( Status.TrackTitle.Value );
            }
            else if ( controlData.CheckAgainst( ConduitControlPacket.CONTROL_CLIENT_READY ) ) {
                client.Ready = true;
            }
            else if ( controlData.CheckAgainst( ConduitControlPacket.CONTROL_CLIENT_NOT_READY ) ) {
                client.Ready = false;
            }
        }
    }
}
