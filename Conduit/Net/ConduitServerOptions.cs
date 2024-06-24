namespace Conduit.Net;

/// <summary>
/// Holds options related to ConduitServer
/// </summary>
public struct ConduitServerOptions {

    /// <summary>
    /// If true, this server will send empty packets instead of filtering them.
    /// </summary>
    public bool SendEmptyPackets = false;

    /// <summary>
    /// Creates a new ConduitServerOptions
    /// </summary>
    /// <param name="sendEmptyPackets"> The value of <see cref="SendEmptyPackets" /> </param>
    public ConduitServerOptions( bool sendEmptyPackets ) => SendEmptyPackets = sendEmptyPackets;

    /// <summary>
    /// Creates a new ConduitServerOptions
    /// </summary>
    public ConduitServerOptions( ) {
    }
}
