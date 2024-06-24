using System.Net;

using Conduit.Net.Turnkey;

namespace ConduitLiveClient;

public partial class ConduitClientForm : Form {

    /// <summary>
    /// The conduit client that will receive Opus data and play to the speakers.
    /// </summary>
    private readonly ConduitTurnkeyClient client;

    /// <summary>
    /// Holds the point that the user clicked on the Title Label at (used for window movement)
    /// </summary>
    private PointF mouseDelta = new( );

    public ConduitClientForm( ) {
        InitializeComponent( );

        //Create client and hook events
        client = new ConduitTurnkeyClient( textboxHost.Text, ushort.Parse( textboxPort.Text ) );
        //Change title to be green on connect
        client.OnConnected += ( object? o, EventArgs e ) => Invoke( ( ) => labelTitle.ForeColor = Color.Green );
        //And red on disconnect
        client.OnDisconnected += ( object? o, EventArgs e ) => Invoke( ( ) => labelTitle.ForeColor = Color.Red );
        //Change track title
        client.OnTrackTitleChanged += ( object? o, EventArgs e ) => Invoke( ( ) => label1.Text = client.CurrentTrackTitle.Value );
        //Copy slider volume to client volume
        sliderVolume.VolumeChanged += ( object? o, EventArgs e ) => client.Volume = sliderVolume.Volume;

#if DEBUG
        Load += ( object? o, EventArgs e ) => new ConduitClientDebugForm( ) { Client = client }.Show( );
#endif

        FormClosed += ( object? o, FormClosedEventArgs e ) => client.Dispose( true );

        buttonClose.Click += ( object? o, EventArgs e ) => Application.Exit( );
    }

    /// Connects to the server </summary> <param name="sender"> Unused </param> <param name="e">
    /// Unused </param>
    private void clickConnect( object sender, EventArgs e ) {
        if ( client.Connected )
            client.Disconnect( );

        IPAddress? addr = ConduitClientFormHelpers.FindIPAddress( textboxHost.Text);

        if ( addr is null ) {
            labelTitle.Text = "Invalid host!";
            return;
        }

        client.ChangeServer( new IPEndPoint( addr, int.Parse( textboxPort.Text ) ) );

        client.Connect( );
    }

    /// <summary>
    /// Runs when the Opacity button is clicked
    /// </summary>
    /// <param name="sender"> Unused </param>
    /// <param name="e">      Unused </param>
    private void clickOpacity( object? sender, EventArgs e )
        => Opacity = Opacity switch {
            0.25 => 1,
            1 => 0.25,
            _ => 1,
        };

    /// <summary>
    /// Calculates the point that the user clicked on the title
    /// </summary>
    /// <param name="sender"> Unused </param>
    /// <param name="e">      Used to make sure the left button is pressed </param>
    private void onTitleMouseDown( object? sender, MouseEventArgs e ) {
        //Make sure the user LEFT clicked
        if ( e.Button != MouseButtons.Left )
            return;
        //Calculate where on the label they clicked
        mouseDelta = new Point( Cursor.Position.X - Location.X, Cursor.Position.Y - Location.Y );
        //Subscribe the MouseMove event
        labelTitle.MouseMove += onTitleMouseMove;
    }

    /// <summary>
    /// Moves the window along with the mouse
    /// </summary>
    /// <param name="sender"> Unused </param>
    /// <param name="e">      Used to make sure the left button is pressed </param>
    private void onTitleMouseMove( object? sender, MouseEventArgs e ) {
        //Make sure the button is left
        if ( e.Button == MouseButtons.Left )
            //Move the form
            Location = new Point( Cursor.Position.X - (int) mouseDelta.X, Cursor.Position.Y - (int) mouseDelta.Y );
        //If the button is released
        else
            //Unsubscribe the event
            labelTitle.MouseMove -= onTitleMouseMove;
    }

    /// <summary>
    /// Updates the meters that show how long is left in the conduit buffers
    /// </summary>
    /// <param name="sender"> Unused </param>
    /// <param name="e">      Unused </param>
    private void tickBufferHealthTimer( object sender, EventArgs e ) {
        if ( client != null ) {
            meterBufferAudio.ValueCoefficient = (float) ( client.AudioBufferPercent / 100f );
            meterSocketHealth.ValueCoefficient = (float) ( client.SocketBufferPercent / 100f );

            meterBufferAudio.Invalidate( );
            meterSocketHealth.Invalidate( );
        }
    }
}
