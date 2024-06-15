using Conduit.Codec;

using NAudio.Wave;

namespace Conduit.Tests;

public class ConduitEncoderTests {
    private ConduitEncoder encoder;

    [Test]
    public void Encode( ) {
        bool frameAvailableRaised = false;
        encoder.OnFrameAvailable += ( object? o, EventArgs e ) => frameAvailableRaised = true;

        byte[] data = getRandomBytes( 5120 );

        encoder.AddSamples( data, 0, 5120 );

        if ( !frameAvailableRaised ) {
            Assert.Fail( "The OnFrameAvailable event was not raised." );
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
