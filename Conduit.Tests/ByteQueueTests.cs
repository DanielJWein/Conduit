using Conduit.Util;

namespace Conduit.Tests;

public class ByteQueueTests {
    private ByteQueue queue;

    [SetUp]
    public void SetUp( ) {
        queue = new( 1024 );
    }

    [Test]
    public void Test( ) {
        byte[] data = TestHelpers.GetRandomBytes(1024);

        //Invalid count
        Assert.Throws<ArgumentOutOfRangeException>( ( ) => queue.Write( data, 0, -1 ),
            "The buffer allowed an invalid count!" );
        Assert.Throws<ArgumentOutOfRangeException>( ( ) => queue.Write( data, 0, 0 ),
            "The buffer allowed an invalid count!" );

        //Invalid offet
        Assert.Throws<ArgumentOutOfRangeException>( ( ) => queue.Write( data, -1, 1 ),
            "The buffer allowed an invalid offset!" );

        queue.Write( data, 0, 1024 );

        if ( queue.QueuedBytes != 1024 ) {
            Assert.Fail( $"The number of bytes did not match the expected value. {queue.QueuedBytes} written, 1024 expected." );
        }

        //Writes past end of buffer
        Assert.Throws<ArgumentOutOfRangeException>( ( ) => queue.Write( data, 0, 1 ),
            "The buffer allowed to write past its end!" );

        queue.Read( data, 0, 4 );

        if ( queue.QueuedBytes != 1020 ) {
            Assert.Fail( $"The number of bytes queued did not match the expectation. {queue.QueuedBytes} queued, 1020 expected." );
        }

        queue.Write( data, 0, 4 );

        byte[] data2 = queue;

        Assert.Pass( "All tests succeeded." );
    }
}
