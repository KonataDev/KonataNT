using System.Security.Cryptography;
using System.Text;
using KonataNT.Utility.Crypto;

namespace KonataNT.Common;

[Serializable]
public class BotKeystore
{
    public BotKeystore() { }
    
    public BotKeystore(uint botUin, string password)
    {
        PasswordMd5 = MD5.HashData(Encoding.UTF8.GetBytes(password));
        Uin = botUin;
    }
    
    public uint Uin { get; set; }
    
    public string Uid { get; set; }
    
    public byte[]? A2 { get; set; }
    
    public DateTime SessionTime { get; set; }

    public byte[] D2 { get; set; } = Array.Empty<byte>();
    
    public byte[] D2Key { get; set; } = Array.Empty<byte>();
    
    public byte[] Tgt { get; set; } = Array.Empty<byte>();
    
    public byte[] Guid { get; set; } = System.Guid.NewGuid().ToByteArray();
    
    public byte[] PasswordMd5 { get; set; } = Array.Empty<byte>();
    
    public BotInfo Info { get; set; } = new();
    
    internal byte[]? UnusualSign { get; set; }
    
    internal byte[]? ExchangeKey { get; set; }
    
    internal string? KeySign { get; set; }
    

    internal EcdhProvider PrimeProvider { get; } = new(EllipticCurve.Prime256V1);
    
    internal EcdhProvider ScepProvider { get; } = new(EllipticCurve.Secp192K1);
}

[Serializable]
public class BotInfo
{
    public string Name { get; set; } = string.Empty;
    
    public byte Age { get; set; }
    
    public byte Gender { get; set; }
}