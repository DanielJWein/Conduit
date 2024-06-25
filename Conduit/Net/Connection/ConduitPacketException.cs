namespace Conduit.Net.Connection;

///<inheritdoc/>
[Serializable]
public class ConduitPacketException : ConduitException {

    ///<inheritdoc/>
    public ConduitPacketException( ) { }

    ///<inheritdoc/>
    public ConduitPacketException( string message ) : base( message ) { }

    ///<inheritdoc/>
    public ConduitPacketException( string message, Exception inner ) : base( message, inner ) { }

    ///<inheritdoc/>
    protected ConduitPacketException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
}
