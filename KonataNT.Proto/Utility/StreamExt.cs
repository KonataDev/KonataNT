namespace KonataNT.Proto.Utility;

public static class StreamExt
{
    public static bool IsAvailable(this Stream stream, int count)
    {
        return stream.Length - stream.Position >= count;
    }
}

