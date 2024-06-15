using System.ComponentModel;

namespace ConduitLiveClient;

public partial class ConduitVolumeSlider : UserControl {
    private readonly Container components;

    private readonly float minDb = -48f;

    private readonly StringFormat stringFormat = new( ) {
        LineAlignment = StringAlignment.Center,
        Alignment = StringAlignment.Center
    };

    private SolidBrush backgroundBrush = new( Color.LightGreen );

    private Pen borderPen = new( Color.Green, 1f );

    private SolidBrush textBrush = new( Color.Black );

    private SolidBrush unfilledBrush = new( Color.Transparent );

    private Color unfilledColor = Color.Transparent,
                backgroundColor = Color.LightGreen,
                borderColor = Color.Green,
                textColor = Color.Black;

    private float volume = 1f;

    public ConduitVolumeSlider( ) {
        DoubleBuffered = true;
        components = new( );
        initializeComponent( );
    }

    /// <summary>
    /// Raised when the volume is changed
    /// </summary>
    public event EventHandler? VolumeChanged;

    /// <summary>
    /// The filled background color
    /// </summary>
    public Color BackgroundColor {
        get => backgroundColor;
        set {
            if ( value != backgroundColor ) {
                backgroundColor = value;
                lock ( backgroundBrush ) {
                    backgroundBrush.Dispose( );
                    backgroundBrush = new( backgroundColor );
                }
            }
        }
    }
    /// <summary>
    /// The color for the border
    /// </summary>
    public Color BorderColor {
        get => borderColor;
        set {
            if ( value != borderColor ) {
                borderColor = value;
                lock ( borderPen ) {
                    borderPen.Dispose( );
                    borderPen = new( borderColor, 1f );
                }
            }
        }
    }
    /// <summary>
    /// The text color
    /// </summary>
    public Color TextColor {
        get => textColor;
        set {
            if ( value != textColor ) {
                textColor = value;
                lock ( textBrush ) {
                    textBrush.Dispose( );
                    textBrush = new( textColor );
                }
            }
        }
    }
    /// <summary>
    /// The background color for the slider
    /// </summary>
    public Color UnfilledColor {
        get => unfilledColor;
        set {
            if ( value != unfilledColor ) {
                unfilledColor = value;
                lock ( unfilledBrush ) {
                    unfilledBrush.Dispose( );
                    unfilledBrush = new( unfilledColor );
                }
            }
        }
    }
    /// <summary>
    /// The volume value in the range of [0, 1]
    /// </summary>
    [DefaultValue( 0.25f )]
    public float Volume {
        get {
            return volume;
        }
        set {
            value = Math.Clamp( value, 0f, 1f );

            if ( volume != value ) {
                volume = value;

                VolumeChanged?.Invoke( this, EventArgs.Empty );

                Invalidate( );
            }
        }
    }
    /// <summary>
    /// Sets up all of the components
    /// </summary>
    private void initializeComponent( ) {
        Name = "sliderVolume";
        Size = new Size( 96, 16 );
        Volume = 0.25f;
    }
    /// <summary>
    /// Sets the volume from the mouse's X position
    /// </summary>
    /// <param name="x">The mouse's X position</param>
    private void setVolumeFromMouse( int x ) {
        float num = (1f - (float)x / Width) * minDb;
        Volume = x <= 0
            ? 0f
            : (float) Math.Pow( 10.0, num / 20f );
    }

    /// <summary>
    /// Disposes this control
    /// </summary>
    /// <param name="disposing">If true, will dispose all sub-components</param>
    protected override void Dispose( bool disposing ) {
        if ( disposing && components != null )
            components.Dispose( );

        base.Dispose( disposing );
    }

    /// <summary>
    /// Changes the volume if the left mouse button is pressed.
    /// </summary>
    protected override void OnMouseDown( MouseEventArgs e ) {
        setVolumeFromMouse( e.X );
        base.OnMouseDown( e );
    }

    /// <summary>
    /// Changes the volume if the left mouse button is pressed.
    /// </summary>
    protected override void OnMouseMove( MouseEventArgs e ) {
        if ( e.Button == MouseButtons.Left )
            setVolumeFromMouse( e.X );

        base.OnMouseMove( e );
    }

    /// <summary>
    /// Draws the control
    /// </summary>
    /// <param name="pe">The paint event args</param>
    protected override void OnPaint( PaintEventArgs pe ) {
        pe.Graphics.DrawRectangle( borderPen, 0, 0, Width - 1, Height - 1 );

        float num = 20f * (float)Math.Log10(Volume);
        float num2 = 1f - num / minDb;

        if ( unfilledColor != Color.Transparent && num2 != 1.0f ) {
            pe.Graphics.FillRectangle( unfilledBrush, 1, 1, ( Width - 2 ) * 1, Height - 2 );
        }

        pe.Graphics.FillRectangle( backgroundBrush, 1, 1, (int) ( ( Width - 2 ) * num2 ), Height - 2 );

        pe.Graphics.DrawString( $"{num:F2} dB ({Volume * 100:n2}%)", Font, textBrush, ClientRectangle, stringFormat );
    }
}
