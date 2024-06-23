using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Conduit.Codec;

namespace Conduit.Tests;

public class ConduitDecoderTests {
    private const string encData = @"/E16UiUU6VPurfhc6cizbSUXpP1TY+khblpbYlNYrCg3nLCXbhPrn9E3sXtBTB65Ilh9sKZCFi0wIzTN7zWMw++YJT8zFE677KmofvIWZeEJTwqWuykxkiThLkfn8CMV";

    private ConduitDecoder decoder;

    [Test]
    public void DecodeEmpty( ) {
        if ( decoder.Buffer.BufferedBytes > 0 ) {
            Assert.Fail( "The buffer was not empty." );
        }
        decoder.DecodeFrame( ConduitCodecFrame.EmptyFrame );
        if ( decoder.Buffer.BufferedBytes > 0 ) {
            Assert.Fail( $"The DecodeFrame method added {decoder.Buffer.BufferedBytes} bytes to the buffer (expected 0)" );
        }
        Assert.Pass( $"{decoder.Buffer.BufferedBytes} were added to the buffer (expected !0)" );
    }

    [Test]
    public void DecodeReal( ) {
        if ( decoder.Buffer.BufferedBytes > 0 ) {
            Assert.Fail( "The buffer was not empty." );
        }
        decoder.DecodeFrame( new( encData ) );
        if ( decoder.Buffer.BufferedBytes <= 0 ) {
            Assert.Fail( "The DecodeFrame method did not add any bytes to the buffer." );
        }
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
