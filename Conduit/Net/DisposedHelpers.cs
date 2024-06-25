using System.Diagnostics;

namespace Conduit.Net;

public static class DisposedHelpers {

    /// <summary>
    /// Throws an exception if this object is disposed
    /// </summary>
    /// <exception cref="ObjectDisposedException"> Thrown if the object is disposed. </exception>
    public static void ThrowIfDisposed( bool disposed ) {
        if ( disposed ) {
            var method = new StackTrace().GetFrame(1).GetMethod();
            var name = method.ReflectedType.Name;
            throw new ObjectDisposedException( name );
        }
    }
}
