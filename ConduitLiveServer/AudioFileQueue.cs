using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace ConduitLiveServer;

/// <summary>
/// Represents an audio file queue
/// </summary>
public class AudioFileQueue : ISampleProvider {

    /// <summary>
    /// Holds the audio files that are open
    /// </summary>
    private readonly List<Tuple<AudioFileReader, ISampleProvider>> afrs = new( );

    /// <summary>
    /// If true, will always provide data
    /// </summary>
    private readonly bool readFully = true;

    public event EventHandler OnNewReaderPlaying;

    /// <summary>
    /// Holds the amount of time left in the queue
    /// </summary>
    public double DurationLeft
        => !afrs.Any( ) ? 0
            : afrs.Sum( x => x.Item1.TotalTime.TotalSeconds ) - afrs.First( ).Item1.CurrentTime.TotalSeconds;

    public string? PlayingFile => afrs.FirstOrDefault( )?.Item1?.FileName;

    /// <summary>
    /// Holds the total duration of the queue
    /// </summary>
    public double TotalDuration
        => !afrs.Any( ) ? 0
            : afrs.Sum( x => x.Item1.TotalTime.TotalSeconds );

    /// <summary>
    /// Holds the wave format files are converted to
    /// </summary>
    public WaveFormat WaveFormat { get; set; } = new WaveFormat( 48000, 2 );

    private void removeAllFinishedReaders( ) {
        int numReaders = afrs.Count;
        while ( ( afrs.FirstOrDefault( )?.Item1?.CurrentTime ?? TimeSpan.Zero )
            >= ( afrs.FirstOrDefault( )?.Item1?.TotalTime ?? TimeSpan.FromSeconds( 1 ) ) )
            BlastTheFirstOne( );
        int newNumReaders = afrs.Count;
        if ( newNumReaders != numReaders )
            OnNewReaderPlaying?.Invoke( this, null );
    }

    internal IEnumerable<AudioFileReader> GetReaders( ) {
        return afrs.Select( x => x.Item1 );
    }

    /// <summary>
    /// Adds an AudioFileReader to the queue
    /// </summary>
    /// <param name="afr"> The file to read </param>
    public void AddReader( AudioFileReader afr ) {
        bool firstReader = afrs.Count == 0;
        ISampleProvider sp = afr;

        if ( afr.WaveFormat.Channels == 1 )
            sp = new StereoToMonoSampleProvider( sp );

        if ( afr.WaveFormat.SampleRate != 48000 )
            sp = new WdlResamplingSampleProvider( sp, 48000 );

        afrs.Add( Tuple.Create( afr, sp ) );
        if ( firstReader )
            OnNewReaderPlaying?.Invoke( this, null );
    }

    /// <summary>
    /// Removes the first item
    /// </summary>
    public void BlastTheFirstOne( ) {
        afrs.RemoveAt( 0 );
    }

    /// <summary>
    /// Clears the queue
    /// </summary>
    public void Clear( ) {
        afrs.Clear( );
    }

    /// <summary>
    /// Reads data into the queue
    /// </summary>
    /// <param name="buffer"> The buffer to read to </param>
    /// <param name="offset"> The offset to read to </param>
    /// <param name="count">  The number of samples to read </param>
    /// <returns> The number of samples read </returns>
    /// <exception cref="EndOfStreamException">
    /// Thrown if ReadFully is false, and there are no readers left.
    /// </exception>
    public int Read( float[ ] buffer, int offset, int count ) {
        lock ( afrs ) {
            removeAllFinishedReaders( );

            Array.Fill( buffer, 0, offset, count );

            return afrs.Any( )
                ? afrs.First( ).Item2.Read( buffer, offset, count )
                : readFully
                    ? count
                    : throw new EndOfStreamException( );
        }
    }

    public void RemoveReaderByFilename( string filename ) {
        afrs.RemoveAll( x => x.Item1.FileName.EndsWith( filename ) );
    }
}
