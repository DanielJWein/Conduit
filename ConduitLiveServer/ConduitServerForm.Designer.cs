namespace ConduitLiveServer;

partial class ConduitServerForm {
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConduitServerForm));
        TitleLabel = new Label( );
        StatusLabel = new Label( );
        buttonListen = new Button( );
        buttonStopListen = new Button( );
        RunningTimeLabel = new Label( );
        QueueRemainingLabel = new Label( );
        ConnectionsLabel = new Label( );
        ConnectionsListBox = new ListBox( );
        buttonClearQueue = new Button( );
        buttonViewQueue = new Button( );
        fillConnections = new System.Windows.Forms.Timer( components );
        InfoTimer = new System.Windows.Forms.Timer( components );
        StatusTimer = new System.Windows.Forms.Timer( components );
        buttonClose = new Button( );
        buttonOpacity = new Button( );
        labelDataUnique = new Label( );
        labelDataTotal = new Label( );
        labelBitrate = new Label( );
        buttonDiscardBuffer = new Button( );
        textBox1 = new TextBox( );
        SuspendLayout( );
        // 
        // TitleLabel
        // 
        TitleLabel.AutoSize = true;
        TitleLabel.Font = new Font( "Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point );
        TitleLabel.ForeColor = SystemColors.Control;
        TitleLabel.Location = new Point( 0, -2 );
        TitleLabel.Name = "TitleLabel";
        TitleLabel.Size = new Size( 222, 32 );
        TitleLabel.TabIndex = 0;
        TitleLabel.Text = "Conduit Live Server";
        TitleLabel.MouseDown +=  onTitleMouseDown ;
        // 
        // StatusLabel
        // 
        StatusLabel.AutoSize = true;
        StatusLabel.ForeColor = SystemColors.Control;
        StatusLabel.Location = new Point( 5, 30 );
        StatusLabel.Name = "StatusLabel";
        StatusLabel.Size = new Size( 45, 15 );
        StatusLabel.TabIndex = 1;
        StatusLabel.Text = "STATUS";
        // 
        // buttonListen
        // 
        buttonListen.FlatStyle = FlatStyle.Flat;
        buttonListen.ForeColor = SystemColors.Control;
        buttonListen.Location = new Point( 5, 48 );
        buttonListen.Name = "buttonListen";
        buttonListen.Size = new Size( 83, 29 );
        buttonListen.TabIndex = 2;
        buttonListen.Text = "Start";
        buttonListen.UseVisualStyleBackColor = true;
        buttonListen.Click +=  onListenClick ;
        // 
        // buttonStopListen
        // 
        buttonStopListen.FlatStyle = FlatStyle.Flat;
        buttonStopListen.ForeColor = SystemColors.Control;
        buttonStopListen.Location = new Point( 94, 48 );
        buttonStopListen.Name = "buttonStopListen";
        buttonStopListen.Size = new Size( 83, 29 );
        buttonStopListen.TabIndex = 3;
        buttonStopListen.Text = "Stop";
        buttonStopListen.UseVisualStyleBackColor = true;
        buttonStopListen.Click +=  onStopListeningClick ;
        // 
        // RunningTimeLabel
        // 
        RunningTimeLabel.AutoSize = true;
        RunningTimeLabel.ForeColor = SystemColors.Control;
        RunningTimeLabel.Location = new Point( 184, 33 );
        RunningTimeLabel.Name = "RunningTimeLabel";
        RunningTimeLabel.Size = new Size( 129, 15 );
        RunningTimeLabel.TabIndex = 4;
        RunningTimeLabel.Text = "Running Time: 00:00:00";
        // 
        // QueueRemainingLabel
        // 
        QueueRemainingLabel.AutoSize = true;
        QueueRemainingLabel.ForeColor = SystemColors.Control;
        QueueRemainingLabel.Location = new Point( 184, 48 );
        QueueRemainingLabel.Name = "QueueRemainingLabel";
        QueueRemainingLabel.Size = new Size( 139, 15 );
        QueueRemainingLabel.TabIndex = 5;
        QueueRemainingLabel.Text = "Queue Duration: 00:00:00";
        // 
        // ConnectionsLabel
        // 
        ConnectionsLabel.AutoSize = true;
        ConnectionsLabel.ForeColor = SystemColors.Control;
        ConnectionsLabel.Location = new Point( 184, 63 );
        ConnectionsLabel.Name = "ConnectionsLabel";
        ConnectionsLabel.Size = new Size( 113, 15 );
        ConnectionsLabel.TabIndex = 6;
        ConnectionsLabel.Text = "1 Clients Connected";
        ConnectionsLabel.TextAlign = ContentAlignment.TopCenter;
        // 
        // ConnectionsListBox
        // 
        ConnectionsListBox.BackColor = SystemColors.WindowFrame;
        ConnectionsListBox.ForeColor = SystemColors.Control;
        ConnectionsListBox.FormattingEnabled = true;
        ConnectionsListBox.ItemHeight = 15;
        ConnectionsListBox.Location = new Point( 184, 81 );
        ConnectionsListBox.Name = "ConnectionsListBox";
        ConnectionsListBox.Size = new Size( 281, 109 );
        ConnectionsListBox.TabIndex = 7;
        ConnectionsListBox.SelectedIndexChanged +=  onSelectedConnectionChanged ;
        // 
        // buttonClearQueue
        // 
        buttonClearQueue.FlatStyle = FlatStyle.Flat;
        buttonClearQueue.ForeColor = SystemColors.Control;
        buttonClearQueue.Location = new Point( 94, 81 );
        buttonClearQueue.Name = "buttonClearQueue";
        buttonClearQueue.Size = new Size( 83, 29 );
        buttonClearQueue.TabIndex = 9;
        buttonClearQueue.Text = "Clear Queue";
        buttonClearQueue.UseVisualStyleBackColor = true;
        // 
        // buttonViewQueue
        // 
        buttonViewQueue.FlatStyle = FlatStyle.Flat;
        buttonViewQueue.ForeColor = SystemColors.Control;
        buttonViewQueue.Location = new Point( 5, 81 );
        buttonViewQueue.Name = "buttonViewQueue";
        buttonViewQueue.Size = new Size( 83, 29 );
        buttonViewQueue.TabIndex = 10;
        buttonViewQueue.Text = "View Queue";
        buttonViewQueue.UseVisualStyleBackColor = true;
        // 
        // fillConnections
        // 
        fillConnections.Interval = 1000;
        fillConnections.Tick +=  tickFillConnections ;
        // 
        // InfoTimer
        // 
        InfoTimer.Enabled = true;
        InfoTimer.Interval = 1000;
        InfoTimer.Tick +=  onInfoTimerTick ;
        // 
        // StatusTimer
        // 
        StatusTimer.Enabled = true;
        StatusTimer.Tick +=  onStatusTimerTick ;
        // 
        // buttonClose
        // 
        buttonClose.FlatAppearance.BorderColor = Color.Red;
        buttonClose.FlatAppearance.MouseDownBackColor = Color.Maroon;
        buttonClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(   255,   128,   128 );
        buttonClose.FlatStyle = FlatStyle.Flat;
        buttonClose.ForeColor = SystemColors.Control;
        buttonClose.Location = new Point( 452, 7 );
        buttonClose.Name = "buttonClose";
        buttonClose.Size = new Size( 17, 16 );
        buttonClose.TabIndex = 18;
        buttonClose.TextAlign = ContentAlignment.TopCenter;
        buttonClose.UseVisualStyleBackColor = false;
        // 
        // buttonOpacity
        // 
        buttonOpacity.FlatStyle = FlatStyle.Flat;
        buttonOpacity.ForeColor = SystemColors.Control;
        buttonOpacity.Location = new Point( 429, 7 );
        buttonOpacity.Name = "buttonOpacity";
        buttonOpacity.Size = new Size( 17, 16 );
        buttonOpacity.TabIndex = 19;
        buttonOpacity.UseVisualStyleBackColor = true;
        buttonOpacity.Click +=  onOpacityClick ;
        // 
        // labelDataUnique
        // 
        labelDataUnique.AutoSize = true;
        labelDataUnique.ForeColor = SystemColors.Control;
        labelDataUnique.Location = new Point( 337, 33 );
        labelDataUnique.Name = "labelDataUnique";
        labelDataUnique.Size = new Size( 114, 15 );
        labelDataUnique.TabIndex = 20;
        labelDataUnique.Text = "Total Data to Send: 0";
        // 
        // labelDataTotal
        // 
        labelDataTotal.AutoSize = true;
        labelDataTotal.ForeColor = SystemColors.Control;
        labelDataTotal.Location = new Point( 337, 48 );
        labelDataTotal.Name = "labelDataTotal";
        labelDataTotal.Size = new Size( 97, 15 );
        labelDataTotal.TabIndex = 21;
        labelDataTotal.Text = "Total Data Sent: 0";
        // 
        // labelBitrate
        // 
        labelBitrate.AutoSize = true;
        labelBitrate.ForeColor = SystemColors.Control;
        labelBitrate.Location = new Point( 337, 63 );
        labelBitrate.Name = "labelBitrate";
        labelBitrate.Size = new Size( 99, 15 );
        labelBitrate.TabIndex = 22;
        labelBitrate.Text = "Average Bitrate: 0";
        // 
        // button1
        // 
        buttonDiscardBuffer.FlatStyle = FlatStyle.Flat;
        buttonDiscardBuffer.ForeColor = SystemColors.Control;
        buttonDiscardBuffer.Location = new Point( 94, 116 );
        buttonDiscardBuffer.Name = "button1";
        buttonDiscardBuffer.Size = new Size( 83, 29 );
        buttonDiscardBuffer.TabIndex = 23;
        buttonDiscardBuffer.Text = "Req. Clear";
        buttonDiscardBuffer.UseVisualStyleBackColor = true;
        // 
        // textBox1
        // 
        textBox1.BackColor = SystemColors.WindowFrame;
        textBox1.BorderStyle = BorderStyle.None;
        textBox1.ForeColor = Color.White;
        textBox1.Location = new Point( 5, 219 );
        textBox1.Name = "textBox1";
        textBox1.Size = new Size( 464, 16 );
        textBox1.TabIndex = 24;
        textBox1.Text = "Currently Playing Song Title";
        // 
        // ConduitServerForm
        // 
        AllowDrop = true;
        AutoScaleDimensions = new SizeF( 7F, 15F );
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = SystemColors.WindowFrame;
        ClientSize = new Size( 477, 239 );
        Controls.Add( textBox1 );
        Controls.Add( buttonDiscardBuffer );
        Controls.Add( labelBitrate );
        Controls.Add( labelDataTotal );
        Controls.Add( labelDataUnique );
        Controls.Add( buttonOpacity );
        Controls.Add( buttonClose );
        Controls.Add( buttonViewQueue );
        Controls.Add( buttonClearQueue );
        Controls.Add( ConnectionsListBox );
        Controls.Add( ConnectionsLabel );
        Controls.Add( QueueRemainingLabel );
        Controls.Add( RunningTimeLabel );
        Controls.Add( buttonStopListen );
        Controls.Add( buttonListen );
        Controls.Add( StatusLabel );
        Controls.Add( TitleLabel );
        DoubleBuffered = true;
        FormBorderStyle = FormBorderStyle.None;
        Icon = (Icon) resources.GetObject( "$this.Icon" );
        Name = "ConduitServerForm";
        Text = "Conduit Server";
        DragDrop +=  onDragDrop ;
        DragEnter +=  onDragEnter ;
        ResumeLayout( false );
        PerformLayout( );
    }

    #endregion

    private Label TitleLabel;
    private Label StatusLabel;
    private Button buttonListen;
    private Button buttonStopListen;
    private Label RunningTimeLabel;
    private Label QueueRemainingLabel;
    private Label ConnectionsLabel;
    private ListBox ConnectionsListBox;
    private Button buttonClearQueue;
    private Button buttonViewQueue;
    private System.Windows.Forms.Timer fillConnections;
    private System.Windows.Forms.Timer InfoTimer;
    private System.Windows.Forms.Timer StatusTimer;
    private Button buttonClose;
    private Button buttonOpacity;
    private Label labelDataUnique;
    private Label labelDataTotal;
    private Label labelBitrate;
    private Button buttonDiscardBuffer;
    private TextBox textBox1;
}
