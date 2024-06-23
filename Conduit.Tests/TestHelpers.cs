namespace Conduit.Tests;

internal static class TestHelpers {

    public static byte[ ] getRandomBytes( int length ) {
        byte[] data = new byte[length];

        Random rnd = new();
        rnd.NextBytes( data );
        return data;
    }
}