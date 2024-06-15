namespace Conduit;

/// <summary>
/// Base class for all exceptions in Conduit
/// </summary>
[Serializable]
public class ConduitException : Exception {

    /// <inheritdoc />
    public ConduitException( ) {
    }

    /// <inheritdoc />
    public ConduitException( string message ) : base( message ) {
    }

    /// <inheritdoc />
    public ConduitException( string message, Exception inner ) : base( message, inner ) {
    }

    /// <inheritdoc />
    protected ConduitException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
}
