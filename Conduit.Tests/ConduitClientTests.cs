using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Conduit.Codec;
using Conduit.Net;

namespace Conduit.Tests;

public class ConduitClientTests {
    private const int WAIT_LONG = 500;

    private ConduitClient client;

    private ConduitServer server;

    [SetUp]
    public void SetUp( ) {
        client = new( "127.0.0.1", 64040 );
        server = new( IPAddress.Loopback, 64040 );
    }

    [TearDown]
    public void TearDown( ) {
        client.Dispose( );
        server.Dispose( );
    }

    [Test]
    public async Task TestConnect( ) {
        _ = server.StartListening( );
        Task t= Task.Run( ( ) => client.Connect( ) );
        Thread.Sleep( WAIT_LONG );
        server.UpdateClients( );
        await t;
        if ( !client.Connected ) {
            Assert.Fail( "The client failed to connect." );
        }
        Thread.Sleep( WAIT_LONG );
        if ( server.Clients.Count != 1 ) {
            Assert.Fail( "The server does not have one client." );
        }
        Assert.Pass( "The client connected successfully." );
    }

    [Test]
    public async Task TestDisconnect( ) {
        _ = server.StartListening( );
        Task t= Task.Run( ( ) => client.Connect( ) );
        Thread.Sleep( WAIT_LONG );
        server.UpdateClients( );
        await t;
        if ( !client.Connected ) {
            Assert.Fail( "The client failed to connect." );
        }
        Thread.Sleep( WAIT_LONG );
        if ( server.Clients.Count != 1 ) {
            Assert.Fail( "The server does not have one client." );
        }

        client.Disconnect( );

        Thread.Sleep( WAIT_LONG );
        server.UpdateClients( );

        if ( server.Clients.Count == 1 ) {
            Assert.Fail( "The server is unaware of the disconnection." );
        }

        Assert.Pass( "Disconnection worked successfully." );
    }

    [Test]
    public async Task TestTrackTitle( ) {
        bool eventRaised = false;

        _ = server.StartListening( );
        server.Status.TrackTitle.Value = "Test Beats vol 112041204.0";
        Task t= Task.Run( ( ) => client.Connect( ) );
        Thread.Sleep( WAIT_LONG );
        server.UpdateClients( );
        await t;

        client.OnTrackTitleChanged += ( object? o, EventArgs e ) => eventRaised = true;
        server.Status.TrackTitle.Value = "Test Beats vol 112041205.0";
        client.Update( );
        Thread.Sleep( WAIT_LONG );

        if ( !eventRaised ) {
            Assert.Fail( "The OnTrackTitleChanged event was not raised." );
        }
    }
}
