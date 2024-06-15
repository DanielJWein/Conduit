namespace Conduit.Net.Exceptions;

/// <summary>
/// Thrown when a server is changed while Conduit is connected
/// </summary>
[Serializable]
public class AlreadyConnectedException : ConduitException {

    /// <inheritdoc />
    public AlreadyConnectedException( ) { }

    /// <inheritdoc />
    public AlreadyConnectedException( string message ) : base( message ) { }

    /// <inheritdoc />
    public AlreadyConnectedException( string message, Exception inner ) : base( message, inner ) { }

    /// <inheritdoc />
    protected AlreadyConnectedException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
}
