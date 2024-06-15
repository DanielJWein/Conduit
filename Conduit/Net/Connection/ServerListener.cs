using System.Threading;
using System.Threading.Tasks;

using Conduit.Net.Events;

namespace Conduit.Net.Connection;

internal class ServerListener( Socket serverSocket ) : IDisposable {

    /// <summary>
    /// Represents a cancellation token source
    /// </summary>
    protected readonly CancellationTokenSource cancellationTokenFactory = new( );

    /// <summary>
    /// Holds the listening task
    /// </summary>
    private Task listeningTask = null;

    /// <summary>
    /// Raised when a new client connects
    /// </summary>
    public event EventHandler<ClientConnectedEventArgs> ClientConnected;

    /// <summary>
    /// Disposes this ServerListener
    /// </summary>
    public void Dispose( ) {
        Dispose( true );
        GC.SuppressFinalize( this );
    }

    /// <summary>
    /// Starts listening for new connections
    /// </summary>
    public async Task StartListening( ) {
        serverSocket.Listen( 32 );
        try {
            listeningTask = Task.Run( ( ) => {
                while ( !cancellationTokenFactory.IsCancellationRequested ) {
                    try {
                        Socket st = serverSocket.Accept();
                        st.SendBufferSize = 8192;
                        IPEndPoint ipep = st.RemoteEndPoint as IPEndPoint;
                        ClientConnected?.Invoke( this, new ClientConnectedEventArgs( ipep, st ) );
                    }
                    catch { }
                }
            }, cancellationTokenFactory.Token
            );

            await listeningTask;
        }
        catch {
        }
    }

    /// <summary>
    /// Stops listening for new connections.
    /// </summary>
    public void StopListening( ) => cancellationTokenFactory.Cancel( );

    /// <summary>
    /// Disposes this ServerListener
    /// </summary>
    /// <param name="disposing"> If true, will dispose managed resources </param>
    /// <remarks> If <paramref name="disposing" /> is false, this function does nothing. </remarks>
    protected async void Dispose( bool disposing ) {
        if ( disposing ) {
            cancellationTokenFactory.Cancel( );
            if ( listeningTask is not null )
                await listeningTask.ConfigureAwait( false );
            listeningTask?.Dispose( );
            cancellationTokenFactory?.Dispose( );
        }
    }
}
