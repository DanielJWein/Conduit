namespace Conduit.Codec;

/// <summary>
/// Represents a frame of Opus data for Conduit
/// </summary>
public sealed class ConduitCodecFrame {

    /// <summary>
    /// Represents an empty frame
    /// </summary>
    private const string emptyFrameData = "No Frame";

    /// <summary>
    /// Creates a new empty frame
    /// </summary>
    public ConduitCodecFrame( ) {
        Data = "";
    }

    /// <summary>
    /// Creates a new frame with specified data
    /// </summary>
    /// <param name="data">   The data of this frame </param>
    /// <param name="length"> The length of the input data (uses data.Length if not given) </param>
    public ConduitCodecFrame( byte[ ] data, int length = -1 ) {
        length = length == -1 ? data.Length : length;
        SendData = new byte[ length ];
        Array.Copy( data, SendData, length );
        Data = Convert.ToBase64String( SendData );
    }

    /// <summary>
    /// Creates a new frame with specified data
    /// </summary>
    /// <param name="data"> The data of this frame </param>
    public ConduitCodecFrame( string data ) {
        Data = data;
        SendData = Convert.FromBase64String( data );
    }

    /// <summary>
    /// Represents an empty frame
    /// </summary>
    public static ConduitCodecFrame EmptyFrame { get; private set; } = new( emptyFrameData );

    /// <summary>
    /// Represents the data stored in this frame
    /// </summary>
    public string Data { get; private set; }

    /// <summary>
    /// Returns true if the frame has no information.
    /// </summary>
    public bool IsEmpty => Data == null || Data.Length == 0 || Data == emptyFrameData || Data.Length == 12 || Data.Length == 4;

    /// <summary>
    /// Represents the data stored in this frame in raw form
    /// </summary>
    public byte[ ] SendData { get; private set; }

    /// <summary>
    /// Gets the length of ShortData
    /// </summary>
    public ushort SendDataLength => (ushort) ( SendData?.Length ?? 0 );

    /// <summary>
    /// Gets the data packet format for this frame
    /// </summary>
    /// <returns> A packet ready to be sent (includes headers and length) </returns>
    public byte[ ] GetPacket( ) {
        byte[] outPacket = new byte[ SendData.Length + 4 ];
        outPacket[ 0 ] = 0x01;
        outPacket[ 1 ] = 0x00;
        Array.Copy( BitConverter.GetBytes( (ushort) SendData.Length ), 0, outPacket, 2, 2 );
        Array.Copy( SendData, 0, outPacket, 4, SendData.Length );

        return outPacket;
    }
}
