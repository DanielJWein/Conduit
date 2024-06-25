using NAudio.Wave;

namespace Conduit.Turnkey;

/// <summary>
/// Wraps Conduit.Codec.AlertingBufferedWaveProvider to implement IWaveProvider
/// </summary>
/// <param name="BWP"> The internal alerting buffered wave provider </param>
public class AlertingBufferedWaveProvider( Conduit.Codec.AlertingBuffer BWP ) : IWaveProvider {
    public NAudio.Wave.WaveFormat WaveFormat => new( BWP.WaveFormat.SampleRate, BWP.WaveFormat.BitsPerSample, BWP.WaveFormat.Channels );

    public int Read( byte[ ] buffer, int offset, int count ) => BWP.Read( buffer, offset, count );
}
