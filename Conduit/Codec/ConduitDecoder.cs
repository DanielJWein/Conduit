namespace Conduit.Codec;

/// <summary>
/// Decodes opus data into base64 pcm
/// </summary>
public class ConduitDecoder : ConduitCodecBase {

    /// <summary>
    /// Holds the decoder
    /// </summary>
    private readonly OpusDecoder opusDec;

    /// <summary>
    /// Creates a new ConduitDecoder
    /// </summary>
    public ConduitDecoder( ) : base( )
        => opusDec = new OpusDecoder( WaveFmt.SampleRate, WaveFmt.Channels );

    /// <summary>
    /// Raised when a frame is decoded.
    /// </summary>
    public event EventHandler OnDecodedFrame;

    /// <summary>
    /// Decodes an opus encoded frame and pushes the samples into the buffer.
    /// </summary>
    public void DecodeFrame( ConduitCodecFrame Frame ) {
        if ( IsDisposed ) {
            return;
        }

        //If the frame is empty or null
        if ( Frame?.IsEmpty ?? true )
            return;

        opusFrame = Convert.FromBase64String( Frame.EncodedData );
        int encL =0;
        try {
            encL = opusDec.Decode( opusFrame,
                                  opusFrame.Length,
                                  pcmFrame,
                                  pcmFrame.Length );
        }
        catch ( OpusException ) {
            return;
        }
        Buffer.AddSamples( pcmFrame, 0, encL );

        OnDecodedFrame?.Invoke( this, EventArgs.Empty );
    }

    /// <inheritdoc />
    public override void Dispose( ) {
        if ( disposed ) {
            return;
        }
        opusDec.Dispose( );
        GC.SuppressFinalize( this );
        base.Dispose( );
    }
}
