namespace KonataNT.Utility.Crypto;

internal partial class EcdhImpl
{
    private readonly EcdhProvider _ecdhProvider;
    
    public CryptMethod Method { get; }

    public CryptId MethodId { get; }

    public byte[] ShareKey { get; private set; }

    public EcdhImpl(CryptMethod method, bool isHash = true)
    {
        var crypt = CurveTable[method]; // Select the curve

        _ecdhProvider = new EcdhProvider(crypt.Curve);
        MethodId = crypt.Id;
        ShareKey = GenerateShared(crypt.PubKey, isHash);
    }

    public byte[] GenerateShared(byte[] bobPublic, bool isHash = true)
    {
        var unpack = _ecdhProvider.UnpackPublic(bobPublic);
        ShareKey = _ecdhProvider.KeyExchange(unpack, isHash);

        return ShareKey;
    }

    public byte[] GetPublicKey(bool compress = true) => _ecdhProvider.PackPublic(compress);
}