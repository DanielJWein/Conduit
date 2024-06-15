using Conduit.Codec.NAudio;

namespace Conduit.Codec;

/// <summary>
/// Represents the base class for a ConduitEncoder or ConduitDecoder.
/// </summary>
public abstract class ConduitCodecBase : IDisposable {

    /// <summary>
    /// Holds the wave format used by Conduit
    /// </summary>
    public static WaveFormat WaveFmt = new( 48000, 16, 2 );

    /// <summary>
    /// The frame size, in milliseconds.
    /// </summary>
    protected readonly float frameDuration;

    /// <summary>
    /// If true, this codec is destroyed and cannot be used.
    /// </summary>
    protected bool disposed;

    /// <summary>
    /// Holds Opus encoded data
    /// </summary>
    protected byte[ ] opusFrame;

    /// <summary>
    /// Holds PCM data
    /// </summary>
    protected byte[ ] pcmFrame;

    /// <summary>
    /// Sets up a new Conduit encoder or decoder
    /// </summary>
    protected ConduitCodecBase( ) {
        disposed = false;

        frameDuration = 20;

        Buffer = new( WaveFmt ) {
            BufferDuration = TimeSpan.FromSeconds( 6 ),
            BufferHighThreshold = TimeSpan.FromSeconds( 5 ),
            BufferLowThreshold = TimeSpan.FromSeconds( 1 ),
            ReadFully = true
        };

        pcmFrame = new byte[ (int) ( frameDuration * 192 /* 192kbps */ ) ];
        createOpusBuffer( );
    }

    /// <summary>
    /// Stores the data for this encoder or decoder.
    /// </summary>
    public AlertingBufferedWaveProvider Buffer { get; private set; }

    /// <summary>
    /// If true, this codec is destroyed and cannot be used.
    /// </summary>
    public bool IsDisposed => disposed;

    /// <summary>
    /// Releases all resources used by this instance.
    /// </summary>
    public virtual void Dispose( ) {
        Dispose( true );
        GC.SuppressFinalize( this );
    }

    /// <summary>
    /// Destroys this object
    /// </summary>
    /// <param name="disposing"> Destroy managed objects too </param>
    protected virtual void Dispose( bool disposing ) {
        if ( disposed )
            return;

        if ( disposing ) {
            disposed = true;
            Buffer.ClearBuffer( );
            Buffer.BufferDuration = TimeSpan.Zero;
            pcmFrame = null;
            opusFrame = null;
        }

        disposed = true;
    }

    private void createOpusBuffer( )
        => opusFrame = new byte[ pcmFrame.Length / 40 ];
}
