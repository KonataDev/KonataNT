using System.Runtime.CompilerServices;

namespace KonataNT.Utility;

public static class DateTimeExt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Epoch(this DateTime time) => (time.Ticks - 621355968000000000) / 10000;
}