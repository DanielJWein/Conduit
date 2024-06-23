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
        byte[] data = TestHelpers.getRandomBytes(1024);

        //Invalid count
        Assert.Throws<ArgumentOutOfRangeException>( ( ) => queue.WriteBytes( data, 0, -1 ), "The buffer allowed an invalid count!" );
        Assert.Throws<ArgumentOutOfRangeException>( ( ) => queue.WriteBytes( data, 0, 0 ), "The buffer allowed an invalid count!" );

        //Invalid offet
        Assert.Throws<ArgumentOutOfRangeException>( ( ) => queue.WriteBytes( data, -1, 1 ), "The buffer allowed an invalid offset!" );

        queue.WriteBytes( data, 0, 1024 );

        //Writes past end of buffer
        Assert.Throws<ArgumentOutOfRangeException>( ( ) => queue.WriteBytes( data, 0, 1 ), "The buffer allowed to write past its end!" );

        queue.ReadBytes( data, 0, 4 );

        queue.WriteBytes( data, 0, 4 );

        Assert.Pass( "All tests succeeded." );
    }
}
