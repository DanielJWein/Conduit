namespace Conduit.Net.Events;

/// <summary>
/// Represents a client disconnection event
/// </summary>
/// <remarks> Creates a new ClientDisconnectedEventArgs </remarks>
/// <param name="address"> The address the client was connected from </param>
public class ClientDisconnectedEventArgs( IPAddress address ) : ConduitEventArgs( ) {

    /// <summary>
    /// The address the client was connected from
    /// </summary>
    public IPAddress Address { get; set; } = address;
}
