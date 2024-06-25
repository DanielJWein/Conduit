using Conduit.Codec;

namespace Conduit.Tests;

public class AlertingBufferedWaveProviderTests {
    private AlertingBuffer abwp;

    [SetUp]
    public void SetUp( ) {
        abwp = new( new WaveFormat( 48000, 16, 2 ), TimeSpan.FromSeconds( 5 ) );
    }

    [Test]
    public void TestAlertingBufferedWave( ) {
        bool raisedOut = false;
        bool raisedLow = false;
        bool raisedHigh = false;

        abwp.OnBufferHigh += ( object? e, EventArgs o ) => raisedHigh = true;
        abwp.OnBufferOut += ( object? e, EventArgs o ) => raisedOut = true;
        abwp.OnBufferLow += ( object? e, EventArgs o ) => raisedLow = true;

        byte[] data = TestHelpers.GetRandomBytes(4096);

        abwp.AddSamples( data, 0, 4096 );

        if ( abwp.BufferedBytes != 4096 ) {
            Assert.Fail( $"Expected 4096 bytes in buffer, got {abwp.BufferedBytes}" );
        }

        while ( true ) {
            abwp.AddSamples( data, 0, Math.Min( 4096, abwp.BufferLength - abwp.BufferedBytes ) );
            if ( abwp.BufferedBytes == abwp.BufferLength )
                break;
        }

        if ( !raisedHigh ) {
            Assert.Fail( "The OnBufferHigh event was not raised." );
        }

        while ( true ) {
            abwp.Read( data, 0, Math.Min( 4096, abwp.BufferedBytes ) );
            if ( abwp.BufferedBytes == 0 ) {
                break;
            }
        }
        if ( !raisedLow ) {
            Assert.Fail( "The OnBufferLow event was not raised." );
        }
        if ( !raisedOut ) {
            Assert.Fail( "The OnBufferOut event was not raised." );
        }

        abwp.AddSamples( data, 0, 4096 );

        abwp.ClearBuffer( );

        if ( abwp.BufferedBytes != 0 ) {
            Assert.Fail( "ClearBuffer() did not clear the buffer." );
        }

        Assert.Pass( "All tests succeeded." );
    }
}
