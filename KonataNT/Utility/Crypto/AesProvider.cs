using System.Security.Cryptography;
using KonataNT.Utility.Generator;

namespace KonataNT.Utility.Crypto;

internal static class AesProvider
{
    public static byte[] Encrypt(byte[] data, byte[] key)
    {
        using var aes = new AesGcm(key, 16);
        var iv = ByteGen.GenRandomBytes(12);
        var tag = new byte[16];
        var cipher = new byte[data.Length];
        aes.Encrypt(iv, data, cipher, tag);
        
        var result = new List<byte>();
        result.AddRange(iv);
        result.AddRange(cipher);
        result.AddRange(tag);
        
        return result.ToArray();
    }
    
    public static byte[] Decrypt(byte[] data, byte[] key)
    {
        using var aes = new AesGcm(key, 16);
        var iv = data[..12];
        var cipher = data[12..^16];
        var tag = data[^16..];
        var result = new byte[data.Length - 28];
        aes.Decrypt(iv, cipher, tag, result);
        
        return result;
    }
}