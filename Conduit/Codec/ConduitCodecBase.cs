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

        FrameDuration = 20;

        Buffer = new( WaveFmt ) {
            BufferDuration = TimeSpan.FromSeconds( 6 ),
            BufferHighThreshold = TimeSpan.FromSeconds( 5 ),
            BufferLowThreshold = TimeSpan.FromSeconds( 1 ),
            ReadFully = true
        };

        pcmFrame = new byte[ (int) ( FrameDuration * 192 /* 192kbps */ ) ];
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
    /// The frame size, in milliseconds.
    /// </summary>
    protected float FrameDuration { private set; get; }

    /// <summary>
    /// Changes the frame duration of this codec.
    /// </summary>
    /// <param name="newFrameDuration">
    /// The new frameDuration. Can be one of (2.5, 5, 10, 20, 60).
    /// </param>
    public void ChangeFrameDuration( float newFrameDuration ) {
        if ( disposed )
            throw new ObjectDisposedException( "this", "This item is disposed." );

        FrameDuration = newFrameDuration;
        pcmFrame = new byte[ (int) ( FrameDuration * 192 ) ];
    }

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
