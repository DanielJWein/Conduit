namespace Conduit.Net.Events;

/// <summary>
/// Represents a client connection event
/// </summary>
/// <remarks> Creates a new ClientConnectedEventArgs </remarks>
/// <param name="address"> The IP Address of the client </param>
/// <param name="socket">  The socket that was opened with the client </param>
public class ClientConnectedEventArgs(
    IPEndPoint address, Socket socket ) : ConduitEventArgs( ) {

    /// <summary>
    /// The IP address of the client
    /// </summary>
    public IPEndPoint Address { get; set; } = address;

    /// <summary>
    /// The socket opened for the client
    /// </summary>
    public Socket Socket { get; set; } = socket;
}
