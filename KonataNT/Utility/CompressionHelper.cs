using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;

namespace KonataNT.Utility;

internal static class CompressionHelper
{
    public static byte[] Deflate(byte[] data)
    {
        using var memoryStream = new MemoryStream();
        using var deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress);
        deflateStream.Write(data, 0, data.Length);
        deflateStream.Close();
        return memoryStream.ToArray();
    }
    
    public static byte[] Inflate(Span<byte> data)
    {
        using var ms = new MemoryStream();
        using var ds = new DeflateStream(ms, CompressionMode.Decompress, true);
        using var os = new MemoryStream();

        ms.Write(data);
        ms.Position = 0;

        ds.CopyTo(os);
        var deflate = new byte[os.Length];
        os.Position = 0;
        os.Read(deflate, 0, deflate.Length);

        return deflate;
    }
    
    public static byte[] ZCompress(byte[] data, byte[]? header = null)
    {
        using var stream = new MemoryStream();
        var deflate = Deflate(data);

        stream.Write(header);
        stream.WriteByte(0x78); // Zlib header
        stream.WriteByte(0xDA); // Zlib header

        stream.Write(deflate.AsSpan());
        
        var checksum = Adler32(data);
        stream.Write(checksum.AsSpan());
        
        return stream.ToArray();
    }
    
    public static byte[] ZCompress(string data, byte[]? header = null) => ZCompress(Encoding.UTF8.GetBytes(data), header);

    public static byte[] ZDecompress(Span<byte> data, bool validate = true)
    {
        var checksum = data[^4..];
        
        var inflate = Inflate(data[2..^4]);
        if (validate) return checksum.SequenceEqual(Adler32(inflate)) ? inflate : throw new Exception("Checksum mismatch");
        return inflate;
    }
    
    private static byte[] Adler32(byte[] data)
    {
        uint a = 1, b = 0;
        foreach (byte t in data)
        {
            a = (a + t) % 65521;
            b = (b + a) % 65521;
        }

        uint hash = (b << 16) | a;
        return BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(hash));
    }
}