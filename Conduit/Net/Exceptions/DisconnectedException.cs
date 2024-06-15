namespace Conduit.Net.Exceptions;

/// <summary>
/// Thrown when a client or server disconnects.
/// </summary>
[Serializable]
public class DisconnectedException : ConduitException
{

    /// <inheritdoc />
    public DisconnectedException() { }

    /// <inheritdoc />
    public DisconnectedException(string message) : base(message) { }

    /// <inheritdoc />
    public DisconnectedException(string message, Exception inner) : base(message, inner) { }

    /// <inheritdoc />
    protected DisconnectedException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
