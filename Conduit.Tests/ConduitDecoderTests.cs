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
    public void Decode( ) {
        decoder.DecodeFrame( new( encData ) );
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
