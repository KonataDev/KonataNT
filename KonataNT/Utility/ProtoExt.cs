using ProtoBuf;

namespace KonataNT.Utility;

public static class ProtoExt
{
    public static byte[] Serialize<T>(this T obj)
    {
        using var stream = new MemoryStream();
        Serializer.Serialize(stream, obj);
        return stream.ToArray();
    }
    
    public static T Deserialize<T>(this byte[] data)
    {
        using var stream = new MemoryStream(data);
        return Serializer.Deserialize<T>(stream);
    }
    
    public static T Deserialize<T>(this ReadOnlySpan<byte> data)
    {
        return Serializer.Deserialize<T>(data);
    }
}