using Conduit.Net.Connection;
using Conduit.Net.Turnkey;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace ConduitLiveServer;

public partial class ConduitServerForm : Form {
    internal readonly AudioFileQueue afq = new( );

    private readonly MixingSampleProvider msp;

    private readonly ConduitTurnkeyServer server;

    private readonly DateTime startTime;

    private bool listening = false;

    private PointF mouseDelta = new( );

    public ConduitServerForm( ) {
        InitializeComponent( );
        msp = new( WaveFormat.CreateIeeeFloatWaveFormat( 48000, 2 ) ) {
            ReadFully = true
        };
        msp.AddMixerInput( afq );
        msp.MixerInputEnded += onMSPDropInput;
        //msp.AddMixerInput( silence );
        startTime = DateTime.Now;
        SampleToWaveProvider wp = new( msp );
        WaveFloatTo16Provider w1p = new( wp );
        //WaveFormatConversionProvider wfp = new( new WaveFormat( 48000, 2 ), wp );
        server = new( w1p );
        fillConnections.Start( );
        StatusLabel.Text = "OK";

        buttonDiscardBuffer.Click += ( object? o, EventArgs e ) => server.Send( ConduitControlPacket.CONTROL_DISCARD_BUFFER );
        afq.OnNewReaderPlaying += ( object? o, EventArgs e ) => Invoke( ( ) => textBox1.Text = server.TrackTitle.Value = afq.PlayingFile.Substring( afq.PlayingFile.LastIndexOf( '\\' ) + 1 ) );
        buttonViewQueue.Click += ( object? o, EventArgs e ) => new ConduitServerQueueForm( this ).Show( );
        buttonClearQueue.Click += ( object? o, EventArgs e ) => afq.Clear( );
        buttonClose.Click += ( object? o, EventArgs e ) => Application.Exit( );
        FormClosed += ( object? o, FormClosedEventArgs e ) => server.Dispose( );
    }

    private void onDragDrop( object? sender, DragEventArgs e ) {
        var data = e.Data?.GetData( DataFormats.FileDrop, true );
        if ( data is null )
            return;
        string rdata = ((string[]) data)[0];
        try {
            afq.AddReader( new AudioFileReader( rdata ) );
        }
        catch {
        }
    }

    private void onDragEnter( object? sender, DragEventArgs e ) {
        bool good = e.Data?.GetFormats()?.Contains(DataFormats.FileDrop) ?? false;
        if ( good )
            e.Effect = DragDropEffects.Link;
    }

    private void onInfoTimerTick( object? sender, EventArgs e ) {
        RunningTimeLabel.Text = $"Running Time: {DateTime.Now - startTime:hh\\:mm\\:ss}";
        QueueRemainingLabel.Text = $"Queue Duration: {TimeSpan.FromSeconds( afq.DurationLeft ):hh\\:mm\\:ss}";
        ConnectionsLabel.Text = $"#{server.Clients.Count}";
    }

    private void onListenClick( object? sender, EventArgs e ) {
        _ = server.StartListening( );
        listening = true;
    }

    private void onMSPDropInput( object? sender, SampleProviderEventArgs e ) {
        try {
            afq.BlastTheFirstOne( );
            msp.AddMixerInput( afq );
        }
        catch { }
    }

    private void onOpacityClick( object? sender, EventArgs e ) {
        Opacity = Opacity switch {
            1 => 0.25,
            0.25 => 1,
            _ => 1
        };
    }

    private void onSelectedConnectionChanged( object? sender, EventArgs e ) {
        string? selectedItem = ConnectionsListBox.SelectedItem?.ToString();
        if ( selectedItem == null )
            return;
        for ( int i = 0; i < server.Clients.Count; i++ ) {
            ConduitConnection? x = server.Clients[ i ];
            if ( x.GetAddress( ).ToString( ) == selectedItem )
                server.Kill( x );
        }
    }

    private void onStatusTimerTick( object? sender, EventArgs e ) {
        StatusLabel.Text = listening
                ? "OK (Listening)"
                : "OK (Stopped)";

        labelDataUnique.Text = "Total Data: " + ConduitServerFormHelpers.CounterToStr( server.DataCounter );
        labelDataTotal.Text = "Total Data Sent: " + ConduitServerFormHelpers.CounterToStr( server.TotalDataCounter );
        labelBitrate.Text = $"Average Bitrate: {server.ExpectedBitrate:n0}";
    }

    private void onStopListeningClick( object? sender, EventArgs e ) {
        server.StopListening( );
        listening = false;
        StatusLabel.Text = "OK (Stopped)";
    }

    private void onTitleMouseDown( object? sender, MouseEventArgs e ) {
        mouseDelta = new Point( Cursor.Position.X - Location.X, Cursor.Position.Y - Location.Y );
        TitleLabel.MouseMove += onTitleMouseMove;
    }

    private void onTitleMouseMove( object? sender, MouseEventArgs e ) {
        if ( e.Button == MouseButtons.Left )
            Location = new Point( Cursor.Position.X - (int) mouseDelta.X, Cursor.Position.Y - (int) mouseDelta.Y );
        else
            TitleLabel.MouseMove -= onTitleMouseMove;
    }

    private void tickFillConnections( object? sender, EventArgs e ) {
        ConnectionsListBox.Items.Clear( );
        foreach ( var x in server.Clients )
            ConnectionsListBox.Items.Add( x.GetAddress( ).ToString( ) );
    }
}
