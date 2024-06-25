using System.Net;
using System.Net.Sockets;

using Conduit.Codec;
using Conduit.Net.Exceptions;
using Conduit.Turnkey;

using NAudio.Wave;

namespace Conduit.Net.Turnkey;

/// <summary>
/// Represents a self-managing Conduit Client.
/// </summary>
public sealed class ConduitTurnkeyClient : ConduitClient, IDisposable {
#if DEBUG

    /// <summary>
    /// (Debug Only) Gets the socket for this client
    /// </summary>
    /// <returns> The socket used by this client. </returns>
    public Socket DebugOnlyGetSocket( ) => Socket;

#endif

    /// <summary>
    /// Holds the decoder for this client
    /// </summary>
    private readonly ConduitDecoder conduitDec;

    /// <summary>
    /// Holds the update timer
    /// </summary>
    private readonly Thread updateThread;

    /// <summary>
    /// Holds the audio output
    /// </summary>
    private readonly WaveOutEvent woes;

    /// <summary>
    /// The thread will continue running while this is true.
    /// </summary>
    private bool continueThread;

    /// <summary>
    /// Holds our buffer in a format NAudio can use to play
    /// </summary>
    private AlertingBufferedWaveProvider abwp;

    /// <summary>
    /// Creates a new ConduitTurnkeyClient
    /// </summary>
    public ConduitTurnkeyClient( string address, ushort port = 32662 ) : base( address, port ) {
        conduitDec = new( );
        conduitDec.Buffer.BufferDuration = TimeSpan.FromSeconds( 10 );
        conduitDec.Buffer.BufferLowThreshold = TimeSpan.FromSeconds( 5 );
        conduitDec.Buffer.OnBufferOut += onBufferOutAsync;

        continueThread = true;

        updateThread = new( tick ) {
            Name = "Conduit Turnkey Client Thread"
        };
        updateThread.Start( );

        woes = new( ) {
            DeviceNumber = -1,
            DesiredLatency = 250,
            NumberOfBuffers = 2,
            Volume = 0.125f
        };
        woes.Init( new AlertingBufferedWaveProvider( conduitDec.Buffer ) );

        _ = Task.Run( fillBufferAndPlay );
    }

    /// <summary>
    /// Changes the server this client points to
    /// </summary>
    /// <param name="newServer"> The new server to connect to </param>
    /// <param name="reconnect">
    /// If true, will disconnect the client and reconnect the client to the new server automatically
    /// </param>
    /// <exception cref="AlreadyConnectedException">
    /// Thrown if the client is connected and <paramref name="reconnect" /> is false.
    /// </exception>
    /// <exception cref="ObjectDisposedException"> Thrown if this client is disposed. </exception>
    public void ChangeServer( IPEndPoint newServer, bool reconnect = false ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        if ( Status.Connected ) {
            if ( reconnect ) {
                Disconnect( );
                ServerEndpoint = newServer;
                Connect( );
            }
            else {
                throw new AlreadyConnectedException( "This client is already connected to a server! Set " + nameof( reconnect ) + " to true to automatically reconnect." );
            }
        }
        else {
            ServerEndpoint = newServer;
        }
    }

    /// <summary>
    /// Represents the percentage of the audio buffer being full
    /// </summary>
    public double AudioBufferPercent => ( (
        conduitDec?.Buffer?.BufferedDuration.TotalSeconds ?? 0 ) /
        conduitDec?.Buffer?.BufferDuration.TotalSeconds ?? 0.1 ) *
        100.0;

    /// <summary>
    /// Holds the playbackState of the WaveOut
    /// </summary>
    public PlaybackState PlaybackState => woes?.PlaybackState ?? PlaybackState.Stopped;

    /// <summary>
    /// Represents the percentage of the socket buffer
    /// </summary>
    public double SocketBufferPercent => ( (
        Socket?.Available ?? 0 ) /
        Socket?.ReceiveBufferSize ?? 0.1 ) *
        100.0;

    /// <summary>
    /// Exposes the volume member of the WaveOutEvent.
    /// </summary>
    public float Volume { get => woes?.Volume ?? -1; set => woes.Volume = value; }

    /// <summary>
    /// Pause playback to buffer more data
    /// </summary>
    /// <param name="sender"> Unused </param>
    /// <param name="e">      Unused </param>
    /// <exception cref="ObjectDisposedException"> Thrown if this client is disposed. </exception>
    private async void onBufferOutAsync( object sender, EventArgs e ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        try {
            woes.Pause( );
            await Task.Delay( (int) conduitDec.Buffer.BufferLowThreshold.TotalMilliseconds );
            woes.Play( );
        }
        catch { }
    }

    /// <summary>
    /// Runs the setup task for the WaveOutEvent
    /// </summary>
    /// <exception cref="ObjectDisposedException"> Thrown if this client is disposed. </exception>
    private async void fillBufferAndPlay( ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        await Task.Delay( (int) conduitDec.Buffer.BufferLowThreshold.TotalMilliseconds );
        woes.Play( );
    }

    /// <summary>
    /// Updates this client
    /// </summary>
    /// <param name="nil"> Not used </param>
    /// <exception cref="ObjectDisposedException"> Thrown if this client is disposed. </exception>
    private void tick( object nil ) {
        while ( continueThread ) {
            DisposedHelpers.ThrowIfDisposed( disposed );
            if ( Status.Connected && !serverConnection.Closed && serverConnection.Ready ) {
                try {
                    Update( );
                    conduitDec.DecodeFrame( GetFrame( ) );
                }
                catch ( DisconnectedException ) {
                    continue;
                }
                //The socket could have been closed by GetData().
                if ( Socket == null )
                    return;
            }
            else {
                Thread.Sleep( 10 );
            }
        }
    }

    /// <inheritdoc />
    /// <exception cref="ObjectDisposedException"> Thrown if this client is disposed. </exception>
    protected override void ClearData( ) {
        DisposedHelpers.ThrowIfDisposed( disposed );
        conduitDec.Buffer.ClearBuffer( );
        base.ClearData( );
    }

    /// <summary>
    /// Destroys this object.
    /// </summary>
    public new void Dispose( bool Disposing ) {
        if ( Disposing ) {
            Disconnect( );
            woes?.Dispose( );
            continueThread = false;
            updateThread.Join( );
            conduitDec?.Dispose( );
            Socket?.Dispose( );
        }
        base.Dispose( );
    }

    /// <summary>
    /// Destroys this object.
    /// </summary>
    public new void Dispose( ) {
        if ( !disposed ) {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}
