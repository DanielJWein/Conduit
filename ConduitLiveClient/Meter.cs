namespace ConduitLiveClient;

public partial class Meter : UserControl {
    private Color foreColor = Color.Red;

    private SolidBrush foreColorBrush;

    private bool vertical = false;

    public Meter( ) {
        DoubleBuffered = true;
        InitializeComponent( );
        foreColorBrush = new SolidBrush( foreColor );
    }

    /// <summary>
    /// The filled color
    /// </summary>
    public new Color ForeColor {
        get => foreColor;
        set {
            foreColor = value;
            lock ( foreColorBrush ) {
                foreColorBrush.Dispose( );
                foreColorBrush = new SolidBrush( foreColor );
            }
        }
    }

    public float ValueCoefficient { get; set; } = 1.0f;

    public bool Vertical {
        get => vertical;
        set => vertical = value;
    }

    protected void OnPaint( object sender, PaintEventArgs e ) {
        e.Graphics.Clear( BackColor );

        if ( !vertical ) {
            e.Graphics.FillRectangle( foreColorBrush, 0, 0, Width * ValueCoefficient, Height );
        }
        else {
            e.Graphics.FillRectangle( foreColorBrush, 0, Height * ( 1 - ValueCoefficient ), Width, Height * ValueCoefficient );
        }
    }
}
