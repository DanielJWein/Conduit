using Conduit.Util;

namespace Conduit.Tests;

public class AlerterTests {

    [Test]
    public void IntAlerter( ) {
        bool eventRaised = false;

        Alerter<int> alerter = new(0);
        alerter.ValueChanged += ( object? o, EventArgs e ) => eventRaised = true;

        alerter.Value = 1;

        if ( !eventRaised ) {
            Assert.Fail( "The event was not raised." );
        }

        int x = 0;

        x = alerter;

        if ( x != 1 ) {
            Assert.Fail( "The value was not converted correctly." );
        }

        Assert.Pass( "The event was raised." );
    }
}
