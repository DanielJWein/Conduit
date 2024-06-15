namespace ConduitLiveServer;

internal static class ConduitServerFormHelpers {

    internal static string CounterToStr( ulong x ) {
        return (double) x switch {
            < 0x400 => $"{x:n2}B",
            < 0x100000 => $"{x / (double) 0x400:n2}KB",
            < 0x40000000 => $"{x / ( (double) 0x100000 ):n2}MB",
            < 0x10000000000 => $"{x / ( (double) 0x40000000 ):n2}GB",
            _ => $"{x:n0}B"
        };
    }
}
