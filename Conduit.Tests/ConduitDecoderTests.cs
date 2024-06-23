using Conduit.Codec;

namespace Conduit.Tests;

public class ConduitDecoderTests {
    private const string EncodedData = @"/E16UiUU6VPurfhc6cizbSUXpP1TY+khblpbYlNYrCg3nLCXbhPrn9E3sXtBTB65Ilh9sKZCFi0wIzTN7zWMw++YJT8zFE677KmofvIWZeEJTwqWuykxkiThLkfn8CMV";

    private ConduitDecoder decoder;

    [Test]
    public void DecodeEmpty( ) {
        //We need to be sure that the buffer is empty to check the result
        if ( decoder.Buffer.BufferedBytes > 0 ) {
            Assert.Fail( "The buffer was not empty." );
        }

        //Actually decode an empty frame
        decoder.DecodeFrame( ConduitCodecFrame.EmptyFrame );

        //We expect that zero bytes were added
        if ( decoder.Buffer.BufferedBytes > 0 ) {
            Assert.Fail( $"The DecodeFrame method added {decoder.Buffer.BufferedBytes} bytes to the buffer (expected 0)" );
        }

        //We passed!
        Assert.Pass( $"{decoder.Buffer.BufferedBytes} were added to the buffer (expected !0)" );
    }

    [Test]
    public void DecodeReal( ) {
        //We need to be sure that the buffer is empty to check the result.
        if ( decoder.Buffer.BufferedBytes > 0 ) {
            Assert.Fail( "The buffer was not empty." );
        }

        //Actually decode the frame
        decoder.DecodeFrame( new( EncodedData ) );

        //We expect that more than zero bytes were added to the buffer.
        if ( decoder.Buffer.BufferedBytes <= 0 ) {
            Assert.Fail( "The DecodeFrame method did not add any bytes to the buffer." );
        }

        //We passed!
        Assert.Pass( $"{decoder.Buffer.BufferedBytes} bytes were added to the buffer (expected 0)" );
    }

    [SetUp]
    public void SetUp( ) {
        decoder = new ConduitDecoder( );
    }

    [TearDown]
    public void TearDown( ) {
        decoder.Dispose( );
    }
}
