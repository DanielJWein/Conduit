using System.Threading;
using System.Threading.Tasks;

using Conduit.Codec;
using Conduit.Net.Exceptions;

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
    private readonly ConduitDecoder conduitDec = new( );

    /// <summary>
    /// Holds the update timer
    /// </summary>
    private readonly Thread updateThread;

    /// <summary>
    /// Holds the audio output
    /// </summary>
    private readonly WaveOutEvent woes = new( );

    /// <summary>
    /// The thread will continue running while this is true.
    /// </summary>
    private bool continueThread = true;

    /// <summary>
    /// Creates a new ConduitTurnkeyClient
    /// </summary>
    public ConduitTurnkeyClient( string address, short port = 32662 ) : base( address, port ) {
        conduitDec.Buffer.BufferDuration = TimeSpan.FromSeconds( 10 );
        conduitDec.Buffer.BufferLowThreshold = TimeSpan.FromSeconds( 5 );
        conduitDec.Buffer.OnBufferOut += onBufferOutAsync;

        updateThread = new( update_thread ) {
            Name = "Conduit Turnkey Client Thread"
        };
        updateThread.Start( );

        woes.DeviceNumber = -1;
        woes.DesiredLatency = 250;
        woes.NumberOfBuffers = 2;
        woes.Volume = 0.125f;
        woes.Init( conduitDec.Buffer );

        _ = Task.Run( setupAsync );
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
    public void ChangeServer( IPEndPoint newServer, bool reconnect = false ) {
        if ( Connected ) {
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
    private async void onBufferOutAsync( object sender, EventArgs e ) {
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
    private async void setupAsync( ) {
        await Task.Delay( (int) conduitDec.Buffer.BufferLowThreshold.TotalMilliseconds );
        woes.Play( );
    }

    /// <summary>
    /// Updates this client
    /// </summary>
    /// <param name="nil"> Not used </param>
    private void update_thread( object nil ) {
        while ( continueThread ) {
            if ( Connected && !serverConnection.Closed ) {
                //Get a frame (this also processes control packets)
                try {
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
    protected override void ClearData( ) {
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
            conduitDec?.Dispose( );
            Socket?.Dispose( );
        }
        Dispose( );
    }
}
