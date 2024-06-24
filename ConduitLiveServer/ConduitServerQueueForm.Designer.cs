namespace ConduitLiveServer;

partial class ConduitServerQueueForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent( ) {
        components =  new System.ComponentModel.Container( ) ;
        TitleLabel =  new Label( ) ;
        queueBox =  new ListBox( ) ;
        label1 =  new Label( ) ;
        label2 =  new Label( ) ;
        refreshTimer =  new System.Windows.Forms.Timer( components ) ;
        buttonOpacity =  new Button( ) ;
        buttonClose =  new Button( ) ;
        label3 =  new Label( ) ;
        queueTimer =  new System.Windows.Forms.Timer( components ) ;
        button1 =  new Button( ) ;
        SuspendLayout( );
        // 
        // TitleLabel
        // 
        TitleLabel.AutoSize =  true ;
        TitleLabel.Font =  new Font( "Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point ) ;
        TitleLabel.ForeColor =  SystemColors.Control ;
        TitleLabel.Location =  new Point( 12, -1 ) ;
        TitleLabel.Name =  "TitleLabel" ;
        TitleLabel.Size =  new Size( 227, 32 ) ;
        TitleLabel.TabIndex =  1 ;
        TitleLabel.Text =  "Conduit Live Queue" ;
        TitleLabel.MouseDown +=  titleLabel_MouseDown ;
        // 
        // queueBox
        // 
        queueBox.BackColor =  SystemColors.WindowFrame ;
        queueBox.ForeColor =  Color.White ;
        queueBox.FormattingEnabled =  true ;
        queueBox.ItemHeight =  15 ;
        queueBox.Location =  new Point( 12, 34 ) ;
        queueBox.Name =  "queueBox" ;
        queueBox.Size =  new Size( 281, 334 ) ;
        queueBox.TabIndex =  2 ;
        // 
        // label1
        // 
        label1.AutoSize =  true ;
        label1.ForeColor =  Color.White ;
        label1.Location =  new Point( 12, 371 ) ;
        label1.Name =  "label1" ;
        label1.Size =  new Size( 149, 15 ) ;
        label1.TabIndex =  3 ;
        label1.Text =  "Number of items in queue:" ;
        // 
        // label2
        // 
        label2.AutoSize =  true ;
        label2.ForeColor =  Color.White ;
        label2.Location =  new Point( 158, 371 ) ;
        label2.Name =  "label2" ;
        label2.Size =  new Size( 13, 15 ) ;
        label2.TabIndex =  4 ;
        label2.Text =  "3" ;
        // 
        // refreshTimer
        // 
        refreshTimer.Enabled =  true ;
        refreshTimer.Tick +=  queueRefreshTimer_Tick ;
        // 
        // buttonOpacity
        // 
        buttonOpacity.FlatStyle =  FlatStyle.Flat ;
        buttonOpacity.ForeColor =  SystemColors.Control ;
        buttonOpacity.Location =  new Point( 253, 12 ) ;
        buttonOpacity.Name =  "buttonOpacity" ;
        buttonOpacity.Size =  new Size( 17, 16 ) ;
        buttonOpacity.TabIndex =  21 ;
        buttonOpacity.UseVisualStyleBackColor =  true ;
        buttonOpacity.Click +=  buttonOpacity_Click ;
        // 
        // buttonClose
        // 
        buttonClose.FlatAppearance.BorderColor =  Color.Red ;
        buttonClose.FlatAppearance.MouseDownBackColor =  Color.Maroon ;
        buttonClose.FlatAppearance.MouseOverBackColor =  Color.FromArgb(     255  ,     128  ,     128   ) ;
        buttonClose.FlatStyle =  FlatStyle.Flat ;
        buttonClose.ForeColor =  SystemColors.Control ;
        buttonClose.Location =  new Point( 276, 12 ) ;
        buttonClose.Name =  "buttonClose" ;
        buttonClose.Size =  new Size( 17, 16 ) ;
        buttonClose.TabIndex =  20 ;
        buttonClose.TextAlign =  ContentAlignment.TopCenter ;
        buttonClose.UseVisualStyleBackColor =  false ;
        buttonClose.Click +=  buttonClose_Click ;
        // 
        // label3
        // 
        label3.AutoSize =  true ;
        label3.ForeColor =  Color.White ;
        label3.Location =  new Point( 12, 386 ) ;
        label3.Name =  "label3" ;
        label3.Size =  new Size( 148, 15 ) ;
        label3.TabIndex =  22 ;
        label3.Text =  "Remaining queue duration" ;
        // 
        // queueTimer
        // 
        queueTimer.Enabled =  true ;
        queueTimer.Interval =  5000 ;
        queueTimer.Tick +=  queueTimer_Tick ;
        // 
        // button1
        // 
        button1.FlatAppearance.BorderColor =  Color.IndianRed ;
        button1.FlatStyle =  FlatStyle.Flat ;
        button1.ForeColor =  SystemColors.Control ;
        button1.Location =  new Point( 276, 374 ) ;
        button1.Name =  "button1" ;
        button1.Size =  new Size( 17, 16 ) ;
        button1.TabIndex =  23 ;
        button1.TextAlign =  ContentAlignment.TopCenter ;
        button1.UseVisualStyleBackColor =  true ;
        button1.Click +=  button1_Click ;
        // 
        // ConduitServerQueueForm
        // 
        AutoScaleDimensions =  new SizeF( 7F, 15F ) ;
        AutoScaleMode =  AutoScaleMode.Font ;
        BackColor =  SystemColors.WindowFrame ;
        ClientSize =  new Size( 305, 412 ) ;
        Controls.Add( button1 );
        Controls.Add( label3 );
        Controls.Add( buttonOpacity );
        Controls.Add( buttonClose );
        Controls.Add( label2 );
        Controls.Add( label1 );
        Controls.Add( queueBox );
        Controls.Add( TitleLabel );
        FormBorderStyle =  FormBorderStyle.None ;
        Name =  "ConduitServerQueueForm" ;
        Text =  "ConduitServerQueueForm" ;
        ResumeLayout( false );
        PerformLayout( );
    }

    #endregion

    private Label TitleLabel;
    private ListBox queueBox;
    private Label label1;
    private Label label2;
    private System.Windows.Forms.Timer refreshTimer;
    private Button buttonOpacity;
    private Button buttonClose;
    private Label label3;
    private System.Windows.Forms.Timer queueTimer;
    private Button button1;
}