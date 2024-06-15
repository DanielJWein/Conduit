using Conduit.Codec;

namespace Conduit.Tests;

public class ConduitCodecFrameTests {
    private ConduitCodecFrame frame = new( );

    public byte[ ] getRandomBytes( int length ) {
        byte[] data = new byte[length];

        Random rnd = new();
        rnd.NextBytes( data );
        return data;
    }

    [Test]
    public void NotEmptyFrameIsNotEmpty( ) {
        byte[] data = getRandomBytes(512);
        frame = new( data );

        if ( frame.IsEmpty ) {
            Assert.Fail( );
        }

        Assert.Pass( );
    }

    [Test]
    public void NotEmptyFrameIsNotEmptyEXTREME( ) {
        for ( int i = 9; i < 256; i++ ) {
            byte[] data = getRandomBytes(i);
            frame = new( data );

            if ( frame.IsEmpty ) {
                Assert.Fail( @$"Frame was called empty when it was {i} bytes long!
Frame Data: {frame.EncodedData}
Frame SendData: {frame.RealData}
Frame SendDataLength: {frame.RealDataLength}
" );
            }
        }
        Assert.Pass( );
    }

    [SetUp]
    public void Setup( ) {
        frame = ConduitCodecFrame.EmptyFrame;
    }

    [Test]
    public void TestCreateWithBase64Data( ) {
        byte[] data = getRandomBytes(512);

        string b64Data = System.Convert.ToBase64String(data);
        frame = new( b64Data );

        if ( frame.RealData.Length != data.Length ) {
            Assert.Fail( "The SendData was the wrong length!" );
        }

        for ( int i = 0; i < data.Length; i++ ) {
            if ( data[ i ] != frame.RealData[ i ] ) {
                Assert.Fail( "The SendData was incorrect!" );
            }
        }
    }

    [Test]
    public void TestCreateWithBinaryData( ) {
        byte[] data = getRandomBytes(512);

        frame = new ConduitCodecFrame( data, 512 );

        if ( frame.RealData.Length != 512 ) {
            Assert.Fail( "The SendData was the wrong length!" );
        }

        for ( int i = 0; i < data.Length; i++ ) {
            if ( data[ i ] != frame.RealData[ i ] ) {
                Assert.Fail( "The SendData was incorrect!" );
            }
        }
    }

    [Test]
    public void TestNewFrameIsEmpty( ) {
        if ( !( new ConduitCodecFrame( ) ).IsEmpty ) {
            Assert.Fail( );
        }
        Assert.Pass( );
    }

    [Test]
    public void TestStaticEmptyFrameIsEmpty( ) {
        if ( !ConduitCodecFrame.EmptyFrame.IsEmpty ) {
            Assert.Fail( );
        }
        Assert.Pass( );
    }
}
