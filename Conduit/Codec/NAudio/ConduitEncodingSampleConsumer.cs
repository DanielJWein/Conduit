namespace Conduit.Codec.NAudio;

/// <summary>
/// Takes data from a WaveProvider and encodes it
/// </summary>
/// <remarks> Creates a new ConduitEncodingSampleConsumer </remarks>
/// <param name="wp"> The wave provider we encode from </param>
public sealed class ConduitEncodingSampleConsumer(IWaveProvider wp) : ConduitEncoder
{
    private DateTime lastFrameTime = DateTime.Now;

    /// <summary>
    /// Gets a frame from the encoder.
    /// </summary>
    /// <returns> </returns>
    public override ConduitCodecFrame GetFrame()
    {
        if (Buffer.BufferedBytes < pcmFrame.Length)
        {
            int i = wp.Read(pcmFrame, 0, pcmFrame.Length);
            AddSamples(pcmFrame, 0, Math.Min(pcmFrame.Length, i));
        }

        return base.GetFrame();
    }

    /// <summary>
    /// Gets the number of frames currently available
    /// </summary>
    /// <returns> A number of frames available </returns>
    public int GetFramesAvailable()
    {
        DateTime now = DateTime.Now;
        //Get the number of milliseconds since the last frame sent
        double timeSinceLast = (now - lastFrameTime).TotalMilliseconds;
        //Get the number of frames that should have elapsed since then
        double frameCount = Math.Floor(timeSinceLast / FrameDuration);
        //Subtract the frames we will send from the time left.
        timeSinceLast -= frameCount * FrameDuration;
        lastFrameTime = now - TimeSpan.FromMilliseconds(timeSinceLast);

        return (int)frameCount;
    }
}
