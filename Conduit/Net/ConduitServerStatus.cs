using Conduit.Util;

namespace Conduit.Net;

/// <summary>
/// Provides status information for a ConduitServer
/// </summary>
public struct ConduitServerStatus {

    /// <summary>
    /// Holds the current track title
    /// </summary>
    public Alerter<string> TrackTitle;

    /// <summary>
    /// Creates a new ConduitServerStatus
    /// </summary>
    public ConduitServerStatus( ) {
        TrackTitle = new Alerter<string>( ) {
            Value = "(Nothing)"
        };
    }

    /// <summary>
    /// Holds the amount of data the server has multicasted (before duplication to clients)
    /// </summary>
    public ulong DataCounter { get; set; } = 0;

    /// <summary>
    /// The approximate bitrate of the server's stream
    /// </summary>
    public double ExpectedBitrate { get; set; } = 1;

    /// <summary>
    /// Holds the amount of data the server has multicasted (after duplication to clients)
    /// </summary>
    public ulong TotalDataCounter { get; set; } = 0;
}
