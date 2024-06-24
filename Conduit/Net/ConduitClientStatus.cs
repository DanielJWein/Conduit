using Conduit.Util;

namespace Conduit.Net;

/// <summary>
/// Holds status information for a ConduitClient
/// </summary>
public struct ConduitClientStatus {

    public ConduitClientStatus( ) {
        Connected = false;
        CurrentTrackTitle = new( "(Nothing)" );
    }

    /// <summary>
    /// Gets whether or not the client is connected
    /// </summary>
    public bool Connected { get; internal set; }

    /// <summary>
    /// Holds the current track title and raises events when it is changed
    /// </summary>
    public Alerter<string> CurrentTrackTitle { get; private set; }
}
