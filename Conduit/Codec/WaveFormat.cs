namespace Conduit.Codec;

/// <summary>
/// Represents a generic wave stream format
/// </summary>
public class WaveFormat {

    /// <summary>
    /// The number of bits in a sample
    /// </summary>
    public int BitsPerSample = 16;

    /// <summary>
    /// The number of channels of samples
    /// </summary>
    public int Channels = 2;

    /// <summary>
    /// The number of samples taken per second
    /// </summary>
    public int SampleRate = 48000;

    /// <summary>
    /// Creates a new 48KHz 2-channel 16bit PCM format
    /// </summary>
    public WaveFormat( ) {
    }

    /// <summary>
    /// Creates a new WaveFormat
    /// </summary>
    /// <param name="samplesPerSecond"> The number of samples per second </param>
    /// <param name="bitsPerSample">    The number of bits per sample </param>
    /// <param name="channels">         The number of sample channels </param>
    public WaveFormat( int samplesPerSecond, int bitsPerSample, int channels ) {
        SampleRate = samplesPerSecond;
        BitsPerSample = bitsPerSample;
        Channels = channels;
    }

    /// <summary>
    /// Gets the number of bytes a second this format uses
    /// </summary>
    public int AverageBytesPerSecond => ( BitsPerSample / 8 ) * SampleRate * Channels;
}
