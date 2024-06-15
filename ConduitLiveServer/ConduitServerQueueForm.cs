namespace ConduitLiveServer;

public partial class ConduitServerQueueForm : Form {
    private readonly ConduitServerForm csf;

    private Point mouseDelta;

    public ConduitServerQueueForm( ConduitServerForm owner ) {
        csf = owner;
        InitializeComponent( );
    }

    private void button1_Click( object sender, EventArgs e ) {
        if ( queueBox.SelectedItem is null )
            return;
        else {
            var v = queueBox.SelectedItem as string;
            csf.afq.RemoveReaderByFilename( v );
        }
    }

    private void buttonClose_Click( object sender, EventArgs e ) {
        Close( );
    }

    private void buttonOpacity_Click( object sender, EventArgs e ) {
        Opacity = Opacity switch {
            1 => 0.25,
            0.25 => 1,
            _ => 1
        };
    }

    private void onTitleMouseMove( object? sender, MouseEventArgs e ) {
        if ( e.Button == MouseButtons.Left )
            Location = new Point( Cursor.Position.X - mouseDelta.X, Cursor.Position.Y - mouseDelta.Y );
        else
            TitleLabel.MouseMove -= onTitleMouseMove;
    }

    private void queueRefreshTimer_Tick( object sender, EventArgs e ) {
        label3.Text = csf.afq.DurationLeft.ToString( "n2" ) + " seconds left in queue";
    }

    private void queueTimer_Tick( object sender, EventArgs e ) {
        queueBox.Items.Clear( );

        var readers = csf.afq.GetReaders();
        foreach ( var l in readers ) {
            queueBox.Items.Add( l.FileName.Substring( l.FileName.LastIndexOf( "\\" ) + 1 ) );
        }
        label2.Text = readers.Count( ).ToString( );
    }

    private void TitleLabel_MouseDown( object sender, MouseEventArgs e ) {
        mouseDelta = new Point( Cursor.Position.X - Location.X, Cursor.Position.Y - Location.Y );
        TitleLabel.MouseMove += onTitleMouseMove;
    }
}
