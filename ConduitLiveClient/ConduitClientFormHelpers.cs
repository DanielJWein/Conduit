using System.Net;

internal static class ConduitClientFormHelpers {

    internal static IPAddress? findIPAddress( string txt ) {
        IPAddress? addr = null;
        if ( txt.Any( c => char.IsLetter( c ) ) ) {
            addr = Dns.GetHostEntry( txt ).AddressList.FirstOrDefault( );
        }

        if ( addr is null ) {
            addr = IPAddress.Parse( txt );
        }

        return addr;
    }
}