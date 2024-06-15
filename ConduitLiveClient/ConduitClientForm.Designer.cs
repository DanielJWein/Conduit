namespace ConduitLiveClient;

partial class ConduitClientForm {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose( bool disposing ) {
        if ( disposing && ( components != null ) ) {
            components.Dispose( );
        }
        base.Dispose( disposing );
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent( ) {
        components = new System.ComponentModel.Container( );
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConduitClientForm));
        textboxHost = new TextBox( );
        textboxPort = new TextBox( );
        buttonConnect = new Button( );
        meterBufferAudio = new Meter( );
        labelBufferHealthMin = new Label( );
        labelBufferHealthMax = new Label( );
        meterSocketHealth = new Meter( );
        BufferHealthTimer = new System.Windows.Forms.Timer( components );
        labelTitle = new Label( );
        sliderVolume = new ConduitVolumeSlider( );
        buttonOpacity = new Button( );
        buttonClose = new Button( );
        label1 = new Label( );
        label2 = new Label( );
        toolTips = new ToolTip( components );
        SuspendLayout( );
        // 
        // textboxHost
        // 
        textboxHost.BackColor = SystemColors.WindowFrame;
        textboxHost.ForeColor = Color.White;
        textboxHost.Location = new Point( 106, 180 );
        textboxHost.Name = "textboxHost";
        textboxHost.Size = new Size( 118, 23 );
        textboxHost.TabIndex = 0;
        textboxHost.Text = "127.0.0.1";
        toolTips.SetToolTip( textboxHost, "Server host (IP or domain name)" );
        // 
        // textboxPort
        // 
        textboxPort.BackColor = SystemColors.WindowFrame;
        textboxPort.ForeColor = Color.White;
        textboxPort.Location = new Point( 228, 180 );
        textboxPort.Name = "textboxPort";
        textboxPort.Size = new Size( 47, 23 );
        textboxPort.TabIndex = 2;
        textboxPort.Text = "32662";
        toolTips.SetToolTip( textboxPort, "Server port (default is 32662)" );
        // 
        // buttonConnect
        // 
        buttonConnect.FlatStyle = FlatStyle.Flat;
        buttonConnect.ForeColor = SystemColors.Control;
        buttonConnect.Location = new Point( 38, 179 );
        buttonConnect.Name = "buttonConnect";
        buttonConnect.Size = new Size( 62, 23 );
        buttonConnect.TabIndex = 4;
        buttonConnect.Text = "Connect";
        buttonConnect.UseVisualStyleBackColor = true;
        buttonConnect.Click +=  clickConnect ;
        // 
        // meterBufferAudio
        // 
        meterBufferAudio.BackColor = Color.FromArgb(   64,   64,   64 );
        meterBufferAudio.Location = new Point( 5, 52 );
        meterBufferAudio.Margin = new Padding( 6 );
        meterBufferAudio.Name = "meterBufferAudio";
        meterBufferAudio.Size = new Size( 10, 150 );
        meterBufferAudio.TabIndex = 5;
        toolTips.SetToolTip( meterBufferAudio, "Audio Buffer Usage (seconds)" );
        meterBufferAudio.ValueCoefficient = 1F;
        meterBufferAudio.Vertical = true;
        // 
        // labelBufferHealthMin
        // 
        labelBufferHealthMin.AutoSize = true;
        labelBufferHealthMin.ForeColor = SystemColors.Control;
        labelBufferHealthMin.Location = new Point( 4, 202 );
        labelBufferHealthMin.Name = "labelBufferHealthMin";
        labelBufferHealthMin.Size = new Size( 25, 15 );
        labelBufferHealthMin.TabIndex = 7;
        labelBufferHealthMin.Text = "0  E";
        // 
        // labelBufferHealthMax
        // 
        labelBufferHealthMax.AutoSize = true;
        labelBufferHealthMax.ForeColor = SystemColors.Control;
        labelBufferHealthMax.Location = new Point( 1, 37 );
        labelBufferHealthMax.Name = "labelBufferHealthMax";
        labelBufferHealthMax.Size = new Size( 28, 15 );
        labelBufferHealthMax.TabIndex = 8;
        labelBufferHealthMax.Text = "10 F";
        labelBufferHealthMax.TextAlign = ContentAlignment.TopRight;
        // 
        // meterSocketHealth
        // 
        meterSocketHealth.BackColor = Color.FromArgb(   64,   64,   64 );
        meterSocketHealth.Location = new Point( 16, 52 );
        meterSocketHealth.Margin = new Padding( 6 );
        meterSocketHealth.Name = "meterSocketHealth";
        meterSocketHealth.Size = new Size( 10, 150 );
        meterSocketHealth.TabIndex = 9;
        toolTips.SetToolTip( meterSocketHealth, "Socket Buffer Usage (MB). Only fills once the audio buffer is full." );
        meterSocketHealth.ValueCoefficient = 1F;
        meterSocketHealth.Vertical = true;
        // 
        // BufferHealthTimer
        // 
        BufferHealthTimer.Enabled = true;
        BufferHealthTimer.Interval = 20;
        BufferHealthTimer.Tick +=  tickBufferHealthTimer ;
        // 
        // labelTitle
        // 
        labelTitle.AutoSize = true;
        labelTitle.FlatStyle = FlatStyle.Popup;
        labelTitle.Font = new Font( "Segoe UI", 14.25F,   FontStyle.Bold  |  FontStyle.Italic , GraphicsUnit.Point );
        labelTitle.ForeColor = SystemColors.Control;
        labelTitle.LiveSetting = System.Windows.Forms.Automation.AutomationLiveSetting.Assertive;
        labelTitle.Location = new Point( 4, 2 );
        labelTitle.MaximumSize = new Size( 217, 32 );
        labelTitle.MinimumSize = new Size( 217, 32 );
        labelTitle.Name = "labelTitle";
        labelTitle.Size = new Size( 217, 32 );
        labelTitle.TabIndex = 13;
        labelTitle.Text = "Conduit Live Client";
        toolTips.SetToolTip( labelTitle, "Click and drag to move window" );
        labelTitle.MouseDown +=  onTitleMouseDown ;
        labelTitle.MouseMove +=  onTitleMouseMove ;
        // 
        // sliderVolume
        // 
        sliderVolume.BackgroundColor = Color.Orchid;
        sliderVolume.BorderColor = Color.Black;
        sliderVolume.Location = new Point( 38, 152 );
        sliderVolume.Name = "sliderVolume";
        sliderVolume.Size = new Size( 238, 21 );
        sliderVolume.TabIndex = 15;
        sliderVolume.TextColor = Color.Black;
        sliderVolume.UnfilledColor = Color.FromArgb(   192,   192,   255 );
        // 
        // buttonOpacity
        // 
        buttonOpacity.FlatStyle = FlatStyle.Flat;
        buttonOpacity.ForeColor = SystemColors.Control;
        buttonOpacity.Location = new Point( 243, 5 );
        buttonOpacity.Name = "buttonOpacity";
        buttonOpacity.Size = new Size( 17, 16 );
        buttonOpacity.TabIndex = 16;
        toolTips.SetToolTip( buttonOpacity, "Increase / Reduce Opacity" );
        buttonOpacity.UseVisualStyleBackColor = true;
        buttonOpacity.Click +=  clickOpacity ;
        // 
        // buttonClose
        // 
        buttonClose.FlatAppearance.BorderColor = Color.Red;
        buttonClose.FlatAppearance.MouseDownBackColor = Color.Maroon;
        buttonClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(   255,   128,   128 );
        buttonClose.FlatStyle = FlatStyle.Flat;
        buttonClose.ForeColor = SystemColors.Control;
        buttonClose.Location = new Point( 266, 5 );
        buttonClose.Name = "buttonClose";
        buttonClose.Size = new Size( 17, 16 );
        buttonClose.TabIndex = 17;
        buttonClose.TextAlign = ContentAlignment.TopCenter;
        toolTips.SetToolTip( buttonClose, "Close" );
        buttonClose.UseVisualStyleBackColor = false;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.ForeColor = SystemColors.Control;
        label1.Location = new Point( 38, 58 );
        label1.Name = "label1";
        label1.Size = new Size( 59, 15 );
        label1.TabIndex = 18;
        label1.Text = "(Nothing)";
        label1.TextAlign = ContentAlignment.TopCenter;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Font = new Font( "Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Point );
        label2.ForeColor = SystemColors.Control;
        label2.Location = new Point( 38, 37 );
        label2.Name = "label2";
        label2.Size = new Size( 62, 21 );
        label2.TabIndex = 19;
        label2.Text = "Playing";
        label2.TextAlign = ContentAlignment.TopCenter;
        // 
        // ConduitClientForm
        // 
        AutoScaleDimensions = new SizeF( 7F, 15F );
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = SystemColors.WindowFrame;
        ClientSize = new Size( 287, 219 );
        ControlBox = false;
        Controls.Add( label2 );
        Controls.Add( label1 );
        Controls.Add( buttonClose );
        Controls.Add( buttonOpacity );
        Controls.Add( sliderVolume );
        Controls.Add( meterSocketHealth );
        Controls.Add( labelBufferHealthMax );
        Controls.Add( labelBufferHealthMin );
        Controls.Add( meterBufferAudio );
        Controls.Add( buttonConnect );
        Controls.Add( textboxPort );
        Controls.Add( textboxHost );
        Controls.Add( labelTitle );
        DoubleBuffered = true;
        FormBorderStyle = FormBorderStyle.None;
        Icon = (Icon) resources.GetObject( "$this.Icon" );
        MaximizeBox = false;
        MaximumSize = new Size( 287, 219 );
        MinimizeBox = false;
        MinimumSize = new Size( 287, 219 );
        Name = "ConduitClientForm";
        SizeGripStyle = SizeGripStyle.Hide;
        Text = "Conduit Client";
        ResumeLayout( false );
        PerformLayout( );
    }

    #endregion

    private TextBox textboxHost;
    private TextBox textboxPort;
    private Button buttonConnect;
    private Meter meterBufferAudio;
    private Label labelBufferHealthMin;
    private Label labelBufferHealthMax;
    private Meter meterSocketHealth;
    private System.Windows.Forms.Timer BufferHealthTimer;
    private Label labelTitle;
    private PictureBox conduitIcon;
    private ConduitVolumeSlider sliderVolume;
    private Button buttonOpacity;
    private Button buttonClose;
    private Label label1;
    private Label label2;
    private ToolTip toolTips;
}
