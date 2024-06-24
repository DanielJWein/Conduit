using System.Text;

using Conduit.Codec;

using NAudio.Wave;

namespace Conduit.Tests;

public class ConduitEncoderTests {
    private const int PCM_FRAME_MAX_SIZE = 60 * 192;

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

        //Main test flag
        bool frameAvailableRaised = false;

        //Hook event
        encoder.OnFrameAvailable += ( object? o, EventArgs e ) => frameAvailableRaised = true;

        //Add some random data to the encoder
        encoder.AddSamples( TestHelpers.GetRandomBytes( PCM_FRAME_MAX_SIZE ), 0, PCM_FRAME_MAX_SIZE );

        //Test that frameAvailableRaised was set.
        if ( !frameAvailableRaised ) {
            Assert.Fail( "The OnFrameAvailable event was not raised." );
        }

        Assert.Pass( "The OnFrameAvailable event was raised." );
    }

    [Test]
    public void GetFrame( ) {
        ConduitCodecFrame? frame = null;
        encoder.OnFrameAvailable += ( object? o, EventArgs e ) => frame = encoder.GetFrame( );
        encoder.AddSamples( TestHelpers.GetRandomBytes( PCM_FRAME_MAX_SIZE ), 0, PCM_FRAME_MAX_SIZE );

        if ( frame is null )
            Assert.Fail( "The frame was null." );
        else if ( frame.RealDataLength < 32 )
            Assert.Fail( "The frame was invalid." );
        else if ( frame.IsEmpty )
            Assert.Fail( "The frame was empty." );
        else
            Assert.Pass( "The frame was valid." );
    }

    [SetUp]
    public void Setup( ) {
        ConduitCodecBase.WaveFmt = new WaveFormat( 48000, 16, 2 );
        encoder = new( );
    }

    [TearDown]
    public void TearDown( ) {
        encoder.Dispose( );
    }
}
