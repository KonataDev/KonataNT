using KonataNT.Utility.Crypto;

namespace KonataNT.Common;

[Serializable]
public class BotKeystore
{
    public byte[]? A2 { get; set; }
    
    public DateTime SessionTime { get; set; }

    public byte[] D2 { get; set; } = Array.Empty<byte>();
    
    public byte[] D2Key { get; set; } = Array.Empty<byte>();
    
    public byte[] Tgt { get; set; } = Array.Empty<byte>();
    
    public byte[] Guid { get; set; } = System.Guid.NewGuid().ToByteArray();
    
    internal byte[]? UnusualSign { get; set; }

    internal EcdhProvider PrimeProvider { get; } = new(EllipticCurve.Prime256V1);
    
    internal EcdhProvider ScepProvider { get; } = new(EllipticCurve.Secp192K1);
}