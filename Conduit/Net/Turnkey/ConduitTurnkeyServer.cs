using System.Threading;

using Conduit.Codec;
using Conduit.Codec.NAudio;

namespace Conduit.Net.Turnkey;

/// <summary>
/// Handles sending data to clients automatically
/// </summary>
public sealed class ConduitTurnkeyServer : ConduitServer, IDisposable {

    /// <summary>
    /// Our encoder
    /// </summary>
    private readonly ConduitEncodingSampleConsumer cesc;

    /// <summary>
    /// The time in milliseconds between update() calls
    /// </summary>
    private readonly int dueTime = 60;

    /// <summary>
    /// Runs the update function
    /// </summary>
    private readonly Thread workerThread;

    /// <summary>
    /// If true, will kill the thread.
    /// </summary>
    private bool killThread = false;

    /// <summary>
    /// Creates a ConduitTurnkeyServer
    /// </summary>
    /// <param name="wpr"> The input audio WaveProvider </param>
    public ConduitTurnkeyServer( IWaveProvider wpr ) : base( ) {
        cesc = new( wpr );
        workerThread = new Thread( update ) {
            Name = "Conduit Turnkey Server"
        };
        workerThread.Start( );
    }

    /// <summary>
    /// Destroys this object
    /// </summary>
    /// <param name="disposing"> If true, will destroy managed objects too </param>
    protected override void Dispose( bool disposing ) {
        if ( disposing ) {
            killThread = true;
            workerThread.Join( );
        }
        Close( );
        cesc.Dispose( );
        base.Dispose( disposing );
    }

    /// <summary>
    /// Sends data to the clients
    /// </summary>
    /// <param name="sender"> Not used </param>
    private void update( object sender ) {
        while ( !killThread ) {
            int framesAvailable = cesc.GetFramesAvailable();
            for ( int i = 0; i < framesAvailable; i++ ) {
                //If so, get one.
                ConduitCodecFrame frame = cesc.GetFrame();
                //Test it for validity
                if ( !frame.IsEmpty )
                    //And send it
                    Send( frame );
            }

            Thread.Sleep( dueTime );
        }
    }
}
