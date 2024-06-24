namespace Conduit.Net.Connection;

/// <summary>
/// Represents a client connected to a Conduit Server
/// </summary>
/// <remarks> Creates a new ConduitConnection </remarks>
/// <param name="socket"> The socket to send data to </param>
/// <param name="who">    The IP Address of the client </param>
public sealed class ConduitConnection( Socket socket, IPEndPoint who ) {
    private object socketLock = new( );

    /// <summary>
    /// If true, this connection is closed and cannot be reused.
    /// </summary>
    public bool Closed { get; private set; } = false;

    /// <summary>
    /// Raised when the connection is disconnected.
    /// </summary>
    public event EventHandler OnDisconnected;

    /// <summary>
    /// Closes this connection
    /// </summary>
    public void Close( ) {
        lock ( socketLock ) {
            if ( Closed )
                return;
            if ( socket is not null )
                socket.Blocking = true;
            try {
                socket?.Send( ConduitControlPacket.CONTROL_DISCONNECT );
            }
            catch {
                //The client already closed the connection.
            }
            socket?.Close( );
            socket?.Dispose( );
            socket = null;
            Closed = true;

            OnDisconnected?.Invoke( this, EventArgs.Empty );
        }
    }

    /// <summary>
    /// Gets the IP address of this client
    /// </summary>
    public IPAddress GetAddress( ) => who.Address;

    /// <summary>
    /// Gets the socket for this connection
    /// </summary>
    public Socket? GetSocket( ) => Closed ? null : socket;

    /// <summary>
    /// Gets a string over the socket connection
    /// </summary>
    /// <returns> The string received from the socket. </returns>
    public string GetString( ) {
        byte[] control = new byte[2];
        //Read the length of the encoded title.
        socket.Receive( control, 2, SocketFlags.None );

        ushort length = BitConverter.ToUInt16 (control, 0);
        byte[] encodedData = new byte[length];
        socket.Receive( encodedData, length, SocketFlags.None );

        string base64string = Encoding.ASCII.GetString( encodedData, 0, length );
        byte[] base64Data = Convert.FromBase64CharArray( base64string.ToCharArray(), 0, base64string.Length );
        string CurrentTrackTitle = Encoding.UTF8.GetString( base64Data, 0, base64Data.Length );
        return CurrentTrackTitle;
    }

    /// <summary>
    /// Receives data from the socket.
    /// </summary>
    /// <param name="length"> The length of data to receive </param>
    /// <param name="peek">   Whether or not to peek the socket. </param>
    /// <returns> </returns>
    public byte[ ] Receive( int length, bool peek = false ) {
        if ( Closed )
            return null;

        byte[] data = new byte[ length ];
        try {
            socket?.Receive( data, peek ? SocketFlags.Peek : SocketFlags.None );
        }
        catch ( SocketException ) {
            Close( );
        }
        return data;
    }

    /// <summary>
    /// Receives data from the socket..
    /// </summary>
    /// <param name="data">   The buffer to read into </param>
    /// <param name="index">  The index to read into the buffer </param>
    /// <param name="length"> The length of data to receive </param>
    /// <param name="peek">   Whether or not to peek the socket. </param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if the write to the array would overrun the buffer.
    /// </exception>
    public void Receive( byte[ ] data, int index, int length, bool peek = false ) {
        if ( Closed )
            return;

        if ( data.Length < index + length ) {
            throw new IndexOutOfRangeException( "The write overruns the buffer!" );
        }

        try {
            socket?.Receive( data, length, peek ? SocketFlags.Peek : SocketFlags.None );
        }
        catch ( SocketException ) {
            Close( );
        }
    }

    /// <summary>
    /// Sends a control packet to the client
    /// </summary>
    /// <param name="ControlPacket"> The control packet to send </param>
    public void SendControlPacket( byte[ ] ControlPacket ) {
        if ( Closed )
            return;

        socket.Send( ControlPacket );
    }

    /// <summary>
    /// Sends a frame of data
    /// </summary>
    public void SendFrame( Codec.ConduitCodecFrame Frame ) {
        if ( Closed )
            return;

        socket.Send( Frame.GetPacket( ) );
    }

    /// <summary>
    /// Sends a string over the socket connection
    /// </summary>
    /// <param name="data"> The string that will be sent. </param>
    public void SendString( string data ) {
        if ( Closed )
            return;

        byte[] titleBytes = Encoding.UTF8.GetBytes( data );
        string titleB64 = Convert.ToBase64String( titleBytes );
        byte[] dat = new byte[2 + titleB64.Length];
        BitConverter.GetBytes( (ushort) titleB64.Length ).CopyTo( dat, 0 );
        Encoding.ASCII.GetBytes( titleB64 ).CopyTo( dat, 2 );
        socket.Send( dat );
    }
}
