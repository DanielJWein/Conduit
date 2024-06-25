using System.Linq;

namespace Conduit.Net.Connection;

/// <summary>
/// Represents various Conduit control codes
/// </summary>
public static class ConduitControlPacket {

    /// <summary>
    /// The client is about to have a buffer overflow.
    /// </summary>
    [Obsolete( "This control packet is used for functionality that has since been removed from Conduit." )]
    public static byte[ ] BUFFER_OVERFLOW_IMMINENT => [ 0x01, 0x11 ];

    /// <summary>
    /// The client had a buffer overflow.
    /// </summary>
    [Obsolete( "This control packet is used for functionality that has since been removed from Conduit." )]
    public static byte[ ] BUFFER_OVERFLOW_OCCURRED => [ 0x01, 0x13 ];

    /// <summary>
    /// The client is about to have a buffer underflow.
    /// </summary>
    [Obsolete( "This control packet is used for functionality that has since been removed from Conduit." )]
    public static byte[ ] BUFFER_UNDERFLOW_IMMINENT => [ 0x01, 0x10 ];

    /// <summary>
    /// The client had a buffer underflow.
    /// </summary>
    [Obsolete( "This control packet is used for functionality that has since been removed from Conduit." )]
    public static byte[ ] BUFFER_UNDERFLOW_OCCURRED => [ 0x01, 0x12 ];

    /// <summary>
    /// The client requests the track title.
    /// </summary>
    public static byte[ ] CONTROL_CLIENT_NOT_READY => [ 0x01, 0x81 ];

    /// <summary>
    /// The client requests the track title.
    /// </summary>
    public static byte[ ] CONTROL_CLIENT_READY => [ 0x01, 0x80 ];

    /// <summary>
    /// The client requests the track title.
    /// </summary>
    public static byte[ ] CONTROL_CLIENT_REQUEST_TRACK_TITLE => [ 0x01, 0x82 ];

    /// <summary>
    /// The client or server disconnected.
    /// </summary>
    public static byte[ ] CONTROL_DATA => [ 0x01, 0x00 ];

    /// <summary>
    /// The client or server disconnected.
    /// </summary>
    public static byte[ ] CONTROL_DISCARD_BUFFER => [ 0x01, 0x14 ];

    /// <summary>
    /// The client or server disconnected.
    /// </summary>
    public static byte[ ] CONTROL_DISCONNECT => [ 0x01, 0x01 ];

    /// <summary>
    /// The server changed the track title.
    /// </summary>
    public static byte[ ] CONTROL_TRACK_TITLE_CHANGED => [ 0x01, 0x22 ];

    /// <summary>
    /// Compares this byte array to another byte array
    /// </summary>
    /// <param name="data">  This byte array </param>
    /// <param name="other"> The other byte array </param>
    /// <returns> True, if their data matches </returns>
    public static bool CheckAgainst( this byte[ ] data, byte[ ] other ) {
        return data.SequenceEqual( other );
    }
}
