namespace Conduit.Codec;

/// <summary>
/// Converts input audio into Opus data
/// </summary>
public class ConduitEncoder : ConduitCodecBase {

    /// <summary>
    /// Encodes opusDec data
    /// </summary>
    private readonly OpusEncoder opusEnc;

    /// <summary>
    /// Creates a new ConduitEncoder
    /// </summary>
    public ConduitEncoder( ) : base( )
        => opusEnc = new OpusEncoder( Application.Audio, WaveFmt.SampleRate, WaveFmt.Channels );

    /// <summary>
    /// One or more frames are available
    /// </summary>
    public bool FrameAvailable => Buffer.BufferedBytes >= pcmFrame.Length;

    /// <summary>
    /// Raised when one or more frames are available
    /// </summary>
    public event EventHandler OnFrameAvailable;

    /// <summary>
    /// Adds samples to be encoded
    /// </summary>
    /// <param name="data">       The buffer to add from </param>
    /// <param name="dataOffset"> The offset to start adding from </param>
    /// <param name="dataLength"> The amount of data to add </param>
    public void AddSamples( byte[ ] data, int dataOffset, int dataLength ) {
        Buffer.AddSamples( data, dataOffset, dataLength );
        if ( FrameAvailable )
            OnFrameAvailable?.Invoke( this, null );
    }

    /// <inheritdoc />
    public override void Dispose( ) {
        opusEnc.Dispose( );
        GC.SuppressFinalize( this );
        base.Dispose( );
    }

    /// <summary>
    /// Gets a base64 opusEnc frame
    /// </summary>
    /// <returns> The encoded frame, or "No Frame" if the buffer wasn't ready. </returns>
    public virtual ConduitCodecFrame GetFrame( ) {
        if ( !FrameAvailable )
            return ConduitCodecFrame.EmptyFrame;

        //Read data into pcmFrame.
        _ = Buffer.Read( pcmFrame, 0, pcmFrame.Length );

        try {
            //Encode that data and return it in Base64
            int encL = opusEnc.Encode(pcmFrame,
                                      pcmFrame.Length,
                                      opusFrame,
                                      opusFrame.Length);

            return new( opusFrame, encL );
        }
        catch {
            //Encoding failed.
            return ConduitCodecFrame.EmptyFrame;
        }
    }
}
