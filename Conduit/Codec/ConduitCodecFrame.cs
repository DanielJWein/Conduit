namespace Conduit.Codec;

/// <summary>
/// Represents a frame of Opus data for Conduit
/// </summary>
public sealed class ConduitCodecFrame {

    /// <summary>
    /// Creates a new empty frame
    /// </summary>
    public ConduitCodecFrame( ) {
        EncodedData = "";
    }

    /// <summary>
    /// Creates a new frame with specified data
    /// </summary>
    /// <param name="data">   The data of this frame </param>
    /// <param name="length"> The length of the input data (uses data.Length if not given) </param>
    public ConduitCodecFrame( byte[ ] data, int length = -1 ) {
        length = length == -1 ? data.Length : length;
        RealData = new byte[ length ];
        Array.Copy( data, RealData, length );
        EncodedData = Convert.ToBase64String( RealData );
    }

    /// <summary>
    /// Creates a new frame with specified data
    /// </summary>
    /// <param name="data"> The data of this frame </param>
    public ConduitCodecFrame( string data ) {
        EncodedData = data;
        RealData = Convert.FromBase64String( data );
    }

    /// <summary>
    /// Represents an empty frame
    /// </summary>
    public static ConduitCodecFrame EmptyFrame { get; private set; } = new( [ (byte) 0 ] );

    /// <summary>
    /// Represents the data stored in this frame
    /// </summary>
    public string EncodedData { get; private set; }

    /// <summary>
    /// Returns true if the frame has no information.
    /// </summary>
    public bool IsEmpty => ( EncodedData?.Length ?? 0 ) == 0 || RealDataLength <= 8;

    /// <summary>
    /// Represents the data stored in this frame in raw form
    /// </summary>
    public byte[ ] RealData { get; private set; }

    /// <summary>
    /// Gets the length of ShortData
    /// </summary>
    public ushort RealDataLength => (ushort) ( RealData?.Length ?? 0 );

    /// <summary>
    /// Gets the data packet format for this frame
    /// </summary>
    /// <returns> A packet ready to be sent (includes headers and length) </returns>
    public byte[ ] GetPacket( ) {
        byte[] outPacket = new byte[ RealData.Length + 4 ];
        outPacket[ 0 ] = 0x01;
        outPacket[ 1 ] = 0x00;
        Array.Copy( BitConverter.GetBytes( (ushort) RealData.Length ), 0, outPacket, 2, 2 );
        Array.Copy( RealData, 0, outPacket, 4, RealData.Length );

        return outPacket;
    }
}
