#if DEBUG
using System.Net.Sockets;
using System.Text;

using Conduit.Net.Turnkey;

namespace ConduitLiveClient;

public partial class ConduitClientDebugForm : Form {
    internal ConduitTurnkeyClient? Client = null;

    /// <summary>
    /// Holds the top 512 bytes of Socket.Available
    /// </summary>
    private byte[ ] data = new byte[ 512 ];

    /// <summary>
    /// Creates the ConduitClientDebugForm
    /// </summary>
    public ConduitClientDebugForm( ) {
        InitializeComponent( );
    }

    /// <summary>
    /// Holds a reference to the socket to view for <see cref="writeSocketTop" />.
    /// </summary>
    internal Socket? Socket => Client.DebugOnlyGetSocket( );

    /// <summary>
    /// Updates the form
    /// </summary>
    /// <param name="sender"> Unused </param>
    /// <param name="e">      Unused </param>
    private void form_tick( object sender, EventArgs e ) {
        writeSocketTop( );
    }

    /// <summary>
    /// Writes the label that shows the first 512 bytes of the Socket data
    /// </summary>
    private void writeSocketTop( ) {
        if ( Socket is not null ) {
            Array.Fill<byte>( data, 0 );

            if ( Socket.Connected && Socket.Available > 0 )
                Socket.Receive( data, Math.Min( 512, Socket.Available ), SocketFlags.Peek );
            else {
                return;
            }

            StringBuilder sb = new();

            for ( int i = 0; i < 32; i++ ) {
                for ( int j = 0; j < 16; j++ ) {
                    sb.Append( Socket.Connected ? $"{data[ ( i * 16 ) + j ]:X2} " : "XX " );
                }

                sb.AppendLine( ( ( i + 1 ) * 16 ).ToString( ) );
            }

            label2.Text = sb.ToString( );
        }
    }
}

#endif
