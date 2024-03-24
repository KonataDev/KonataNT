using System.Text;

namespace KonataNT.Utility;

public static class ByteExt
{
    public static string Hex(this byte[] bytes)
    {
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            sb.AppendFormat("{0:x2}", b);
        }

        return sb.ToString();
    }
    
    public static byte[] UnHex(this string hex)
    {
        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = byte.Parse(hex.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
        }

        return bytes;
    }
}