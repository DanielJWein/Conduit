namespace Conduit.Util;

/// <summary>
/// Represents a queue of byte information
/// </summary>
public sealed class ByteQueue {

    /// <summary>
    /// The capacity of this buffer
    /// </summary>
    public readonly int Capacity;

    /// <summary>
    /// The buffer that holds the information
    /// </summary>
    private readonly byte[ ] buffer;

    /// <summary>
    /// The current write pointer
    /// </summary>
    private int writePtr;

    /// <summary>
    /// Creates a new ByteQueue
    /// </summary>
    /// <param name="capacity"> The capacity of the queue </param>
    public ByteQueue( int capacity ) {
        Capacity = capacity;
        buffer = new byte[ Capacity ];
    }

    /// <summary>
    /// Allows indexing into the underlying buffer
    /// </summary>
    /// <param name="x"> The index to read or write </param>
    /// <returns> The byte if this was a read, nothing if this was a write </returns>
    public byte this[ int x ] {
        get => buffer[ x ];
        set => buffer[ x ] = value;
    }

    /// <summary>
    /// Implicit operator to expose the underlying buffer (and allow this to be used as a byte array)
    /// </summary>
    /// <param name="x"> The bytequeue to convert </param>
    public static implicit operator byte[ ]( ByteQueue x ) => x.buffer;

    /// <summary>
    /// Clears the buffer
    /// </summary>
    public void Clear( ) => Array.Clear( buffer, 0, buffer.Length );

    /// <summary>
    /// Reads bytes from this buffer, automatically updating the position
    /// </summary>
    /// <param name="outputBuffer"> The buffer to read to </param>
    /// <param name="offset">       The offset to read to </param>
    /// <param name="count">        The number of bytes to read </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the requested indices exit the target buffer OR if <paramref name="count" /> is
    /// less than 1 OR if <paramref name="offset" /> was less than 0
    /// </exception>
    public void ReadBytes( byte[ ] outputBuffer, int offset, int count ) {
        if ( count < 1 )
            throw new ArgumentOutOfRangeException( nameof( count ), "Count was not above 0!" );

        if ( offset < 0 )
            throw new ArgumentOutOfRangeException( nameof( count ), "The offset was negative!" );

        if ( outputBuffer.Length < offset + count )
            throw new ArgumentOutOfRangeException( nameof( count ), "The requested offset and size overrun the buffer!" );

        //Copy data to consumer array
        Array.Copy( buffer, 0, outputBuffer, offset, count );
        //Consume data from our array
        Array.Copy( buffer, writePtr, buffer, 0, Capacity - writePtr );

        writePtr -= count;
    }

    /// <summary>
    /// Writes bytes into this buffer, automatically updating the position
    /// </summary>
    /// <param name="bytes">  The buffer to read from </param>
    /// <param name="offset"> The offset to read from </param>
    /// <param name="count">  The number of bytes to read </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the requested indices exit the internal buffer OR if <paramref name="count" /> is
    /// less than 1 OR if <paramref name="offset" /> was less than 0
    /// </exception>
    public void WriteBytes( byte[ ] bytes, int offset, int count ) {
        if ( count < 1 )
            throw new ArgumentOutOfRangeException( nameof( count ), "Count was not above 0!" );

        if ( offset < 0 )
            throw new ArgumentOutOfRangeException( nameof( count ), "The offset was negative!" );

        if ( writePtr + count > Capacity )
            throw new ArgumentOutOfRangeException( nameof( count ), "There is not enough capacity to handle the write." );

        Array.Copy( bytes, offset, buffer, writePtr, count );
        writePtr += count;
    }
}
