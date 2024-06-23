using NAudio.Utils;

namespace Conduit.Codec.NAudio;

/// <summary>
/// Provides a buffer that automatically raises events related to the amount of audio left
/// </summary>
/// <remarks>
/// Code was retrofitted from a decompilation of BufferedWaveProvider by Mark Heath of the NAudio
/// Project. See https://github.com/naudio/NAudio for more details.
/// </remarks>
public sealed class AlertingBufferedWaveProvider : IWaveProvider {

    /// <summary>
    /// The underlying CircularBuffer length
    /// </summary>
    private int bufferLength = 1;

    /// <summary>
    /// The underlying CircularBuffer
    /// </summary>

    private CircularBuffer circularBuffer;

    /// <summary>
    /// Creates a new AlertingBufferedWaveProvider
    /// </summary>
    /// <param name="waveFormat"> The format for this object. </param>
    /// <param name="ts">        
    /// The duration of the buffer. If it is null or not provided, it is set to 30 seconds.
    /// </param>
    public AlertingBufferedWaveProvider( WaveFormat waveFormat, TimeSpan? ts = null ) {
        if ( ts is null ) {
            ts = TimeSpan.FromSeconds( 30 );
        }
        WaveFormat = waveFormat;
        BufferLength = waveFormat.AverageBytesPerSecond * 30;
        ReadFully = true;
        BufferLowThreshold = TimeSpan.FromSeconds( ts.Value.TotalSeconds * 0.10f );
        BufferHighThreshold = TimeSpan.FromSeconds( ts.Value.TotalSeconds * 0.90f );
    }

    /// <summary>
    /// Holds the duration of the buffer
    /// </summary>
    public TimeSpan BufferDuration {
        get => TimeSpan.FromSeconds( BufferLength / (double) WaveFormat.AverageBytesPerSecond );
        set => BufferLength = (int) ( value.TotalSeconds * WaveFormat.AverageBytesPerSecond );
    }

    /// <summary>
    /// Holds the current number of bytes buffered
    /// </summary>
    public int BufferedBytes => circularBuffer?.Count ?? 0;

    /// <summary>
    /// Holds the duration of currently buffered audio
    /// </summary>
    public TimeSpan BufferedDuration => TimeSpan.FromSeconds( BufferedBytes / (double) WaveFormat.AverageBytesPerSecond );

    /// <summary>
    /// Represents the buffer duration to emit the BufferHigh event
    /// </summary>
    public TimeSpan BufferHighThreshold { get; set; }

    /// <summary>
    /// Holds the buffer's length
    /// </summary>
    public int BufferLength {
        get => bufferLength;
        private set {
            if ( value == 0 ) {
                circularBuffer?.Reset( );
                circularBuffer = null;
                bufferLength = value;
            }
            else if ( value != bufferLength ) {
                CircularBuffer newCircularBuffer = new(value);
                if ( circularBuffer != null ) {
                    byte[] data = new byte[circularBuffer.Count];
                    circularBuffer.Read( data, 0, circularBuffer.Count );
                    newCircularBuffer.Write( data, 0, Math.Min( circularBuffer.Count, value ) );
                    circularBuffer.Reset( );
                }
                circularBuffer = newCircularBuffer;
                bufferLength = value;
            }
        }
    }

    /// <summary>
    /// Represents the buffer duration to emit the BufferLow event
    /// </summary>
    public TimeSpan BufferLowThreshold { get; set; }

    /// <summary>
    /// If true, the buffer will discard data above the threshold quietly
    /// </summary>
    public bool DiscardOnBufferOverflow { get; set; }

    /// <summary>
    /// If true, this will always provide the number of bytes requested, even if we have to extend
    /// the audio with silence
    /// </summary>
    public bool ReadFully { get; set; }

    /// <summary>
    /// The wave format for this AlertingBufferedWaveProvider
    /// </summary>
    public WaveFormat WaveFormat { get; private set; }

    /// <summary>
    /// Emitted when the buffered duration is too high
    /// </summary>
    public event EventHandler OnBufferHigh;

    /// <summary>
    /// Emitted when the buffered duration is too low
    /// </summary>
    public event EventHandler OnBufferLow;

    /// <summary>
    /// Emitted when the buffered duration is zero
    /// </summary>
    public event EventHandler OnBufferOut;

    /// <summary>
    /// Adds samples to this buffer
    /// </summary>
    /// <param name="buffer"> The buffer to add from </param>
    /// <param name="offset"> The offset to add from </param>
    /// <param name="count">  The number of samples to add </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the buffer fills and DiscardOnBufferOverflow is false
    /// </exception>
    public void AddSamples( byte[ ] buffer, int offset, int count ) {
        circularBuffer ??= new CircularBuffer( BufferLength );

        evokeFillEvents( );

        if ( circularBuffer.Write( buffer, offset, count ) < count && !DiscardOnBufferOverflow )
            throw new InvalidOperationException( "Buffer full" );
    }

    /// <summary>
    /// Clears this buffer
    /// </summary>
    public void ClearBuffer( )
        => circularBuffer?.Reset( );

    /// <summary>
    /// Reads data from this buffer
    /// </summary>
    /// <param name="buffer"> The buffer to read to </param>
    /// <param name="offset"> The offset to read to </param>
    /// <param name="count">  The number of bytes to read </param>
    /// <returns> </returns>
    public int Read( byte[ ] buffer, int offset, int count ) {
        int num = circularBuffer?.Read(buffer, offset, count) ?? 0;

        if ( ReadFully && num < count ) {
            Array.Clear( buffer, offset + num, count - num );
            num = count;
        }
        evokeDepletionEvents( );

        return num;
    }

    private void evokeDepletionEvents( ) {
        if ( BufferedDuration < BufferLowThreshold )
            OnBufferLow?.Invoke( this, EventArgs.Empty );

        if ( BufferedDuration == TimeSpan.Zero )
            OnBufferOut?.Invoke( this, EventArgs.Empty );
    }

    private void evokeFillEvents( ) {
        if ( BufferedDuration > BufferHighThreshold )
            OnBufferHigh?.Invoke( this, EventArgs.Empty );
    }
}
