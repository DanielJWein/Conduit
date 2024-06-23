using Conduit.Codec;

using NAudio.Wave;

namespace Conduit.Tests;

public class ConduitEncoderTests {
    private ConduitEncoder encoder;

    [Test]
    public void Dispose( ) {
        encoder.Dispose( );

        Assert.Throws<ObjectDisposedException>( ( ) => {
            encoder.AddSamples( [ 125, 255 ], 0, 2 );
        } );
    }

    [Test]
    public void Encode( ) {
        //Make size the maximum possible Opus size so that it will always emit at LEAST one frame.
        const int size = 60 * 192;

        //Main test flag
        bool frameAvailableRaised = false;

        //Hook event
        encoder.OnFrameAvailable += ( object? o, EventArgs e ) => frameAvailableRaised = true;

        //Add some random data to the encoder
        encoder.AddSamples( getRandomBytes( size ), 0, size );

        //Test that frameAvailableRaised was set.
        if ( !frameAvailableRaised ) {
            Assert.Fail( "The OnFrameAvailable event was not raised." );
        }

        Assert.Pass( "The OnFrameAvailable event was raised." );
    }

    [Test]
    public void GetFrame( ) {
        bool frameAvailableRaised = false;
        encoder.OnFrameAvailable += ( object? o, EventArgs e ) => frameAvailableRaised = true;

        byte[] data = getRandomBytes( 5120 );

        encoder.AddSamples( data, 0, 5120 );

        if ( !frameAvailableRaised ) {
            Assert.Fail( "The OnFrameAvailable event was not raised." );
        }
        else {
            var frame = encoder.GetFrame( );
            if ( frame.RealDataLength <= 32 ) {
                Assert.Fail( "The frame was invalid." );
            }
            Assert.Pass( "The frame was valid." );
        }
    }

    public byte[ ] getRandomBytes( int length ) {
        byte[] data = new byte[length];

        Random rnd = new();
        rnd.NextBytes( data );
        return data;
    }

    [SetUp]
    public void Setup( ) {
        ConduitCodecBase.WaveFmt = new WaveFormat( 48000, 16, 2 );
        encoder = new( );
    }
}
