using System.Text;
using KonataNT.Common;
using KonataNT.Core.Packet;
using KonataNT.Events;
using KonataNT.Utility.Binary;
using KonataNT.Utility.Crypto;

namespace KonataNT.Core;

/// <summary>
/// The BaseClient that implements most basic login protocol.
/// </summary>
public class BaseClient
{
    private const string Tag = nameof(BaseClient);
    
    public string BotName => KeyStore.Info.Name;

    public uint BotUin => KeyStore.Uin;
    
    internal BotKeystore KeyStore { get; init; }
    
    internal BotAppInfo AppInfo { get; init; }
    
    internal BotConfig Config { get; init; }
    
    internal EventEmitter EventEmitter { get; init; }
    
    internal PacketHandler PacketHandler { get; init; }
    
    internal ILogger Logger { get; init; }
    
    internal BaseClient(BotKeystore keystore, BotConfig config)
    {
        KeyStore = keystore;
        AppInfo = BotAppInfo.ProtocolToAppInfo[config.Protocol];
        Config = config;
        EventEmitter = new EventEmitter();
        PacketHandler = new PacketHandler(this);
        Logger = config.Logger ?? new DefaultLogger(EventEmitter);
    }
    
    public async Task<(string Url, byte[] Image)?> FetchQrCode()
    {
        await PacketHandler.Connect();
        
        var tlv = new TlvPacker(KeyStore, AppInfo);
        var body = new BinaryPacket()
            .WriteUshort(0)
            .WriteUlong(0)
            .WriteByte(0)
            .WritePacket(tlv.PackUp([0x16, 0x1b, 0x1d, 0x33, 0x35, 0x66, 0xd1], true))
            .WriteByte(3);
        var code2d = BuildCode2dPacket(0x31, body.ToArray());
        var login = BuildWtLoginPacket("wtlogin.trans_emp", code2d.ToArray());
        var response = await PacketHandler.SendPacket("wtlogin.trans_emp", login.ToArray());
        
        var decrypted = TeaProvider.Decrypt(response.AsSpan()[16..^1], KeyStore.ScepProvider.ShareKey);
        var reader = new BinaryPacket(decrypted);
        
        reader.Skip(54);
        byte retCode = reader.ReadByte();
        if (retCode != 0)
        {
            Logger.LogError(Tag, $"Failed to fetch QR code, retCode: {retCode}");
            return null;
        }
        KeyStore.QrSign = reader.ReadBytes(Prefix.Uint16 | Prefix.LengthOnly).ToArray();
        
        var tlvBody = new TlvUnPacker(reader);
        var proto = tlvBody.TlvMap[0xD1];
        var image = tlvBody.TlvMap[0x17];

        string url = "";
        return (url, image);
    }

    public async Task<QrCodeState> QueryQrCodeState()
    {
        if (KeyStore.QrSign is null)
        {
            Logger.LogFatal(Tag, "Please fetch QR code first.");
            return QrCodeState.Invalid;
        }

        var body = new BinaryPacket()
            .WriteBytes(KeyStore.QrSign, Prefix.Uint16 | Prefix.LengthOnly)
            .WriteUlong(0) // const 0
            .WriteUint(0) // const 0
            .WriteByte(0) // const 0
            .WriteByte(0x03);         // packet end
        
        var code2d = BuildCode2dPacket(0x12, body.ToArray());
        var login = BuildWtLoginPacket("wtlogin.trans_emp", code2d.ToArray());
        var response = await PacketHandler.SendPacket("wtlogin.trans_emp", login.ToArray());
        
        var decrypted = TeaProvider.Decrypt(response.AsSpan()[16..^1], KeyStore.ScepProvider.ShareKey);
        var reader = new BinaryPacket(decrypted);
        
        uint length = reader.ReadUint();
        reader.Skip(4);
        ushort cmd = reader.ReadUshort();
        reader.Skip(40);
        uint appId = reader.ReadUint();
        var state = (QrCodeState)reader.ReadByte();
        
        Logger.LogInformation(Tag, $"QR Code State: {state}");
        
        if (state == QrCodeState.Confirmed)
        {
            var tlvBody = new TlvUnPacker(reader);
            var tgtgtKey = tlvBody.TlvMap[0x1E];
            var tempPwd = tlvBody.TlvMap[0x18];
            var noPicSig = tlvBody.TlvMap[0x19];
        }
        
        return state;
    }

    public async Task QrCodeLogin()
    {
        
    }

    public async Task Login(Memory<byte> credentials, CredentialType type)
    {
        if (KeyStore is not { ExchangeKey: not null, KeySign: not null })
        {
            Logger.LogFatal(Tag, "Please exchange key first.");
            return;
        }
        
        string command = type switch
        {
            CredentialType.EasyLogin => "trpc.login.ecdh.EcdhService.SsoNTLoginEasyLogin",
            CredentialType.UnusualEasyLogin => "trpc.login.ecdh.EcdhService.SsoNTLoginEasyLoginUnusualDevice",
            CredentialType.PasswordLogin => "trpc.login.ecdh.EcdhService.SsoNTLoginPasswordLogin",
            CredentialType.UnusualPasswordLogin => "trpc.login.ecdh.EcdhService.SsoNTLoginPasswordLoginUnusualDevice",
            CredentialType.NewDeviceLogin => "trpc.login.ecdh.EcdhService.SsoNTLoginPasswordLoginNewDevice",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    
    /// <summary>
    /// Called when Bot is online through login, or reconnected / session resumed.
    /// </summary>
    public async Task BotOnline()
    {
        
    }

    #region Private Builders

    private BinaryPacket BuildCode2dPacket(int cmd, byte[] buffer) => new BinaryPacket()
        .WriteByte(0)
        .WriteUshort((ushort)(53 + buffer.Length))
        .WriteUint((uint)AppInfo.AppId)
        .WriteUint(0x72)
        .WriteBytes(new byte[3])
        .WriteUint((uint)DateTimeOffset.Now.ToUnixTimeSeconds())
        .WriteByte(0x02)
        
        .WriteUshort((ushort)(49 + buffer.Length)) // actually this is manually calculated, real implementation is seen at MiraiGo
        .WriteUshort((ushort)cmd)
        .WriteBytes(new byte[21].AsSpan())
        .WriteByte(3)
        .WriteUint(50)
        .WriteBytes(new byte[14].AsSpan())
        .WriteUint((uint)AppInfo.AppId)
        .WriteBytes(buffer);

    private BinaryPacket BuildWtLoginPacket(string cmd, byte[] buffer)
    {
        var encrypted = TeaProvider.Encrypt(buffer.AsSpan(), KeyStore.ScepProvider.ShareKey.AsSpan());
        var random = new byte[16];
        Random.Shared.NextBytes(random);
        
        var writer = new BinaryPacket()
            .WriteUshort(8001)
            .WriteUshort((ushort)(cmd == "wtlogin.login" ? 2064 : 2066))
            .WriteUshort(0)
            .WriteUint(KeyStore.Uin)
            .WriteByte(3)
            .WriteByte(135)
            .WriteUint(0)
            .WriteByte(19)
            .WriteUshort(0)
            .WriteUshort(AppInfo.AppClientVersion)
            .WriteUint(0)
            .WriteByte(1)
            .WriteByte(1)
            .WriteBytes(random.AsSpan())
            .WriteUshort(0x102)
            .WriteBytes(KeyStore.ScepProvider.GetPublicKey(), Prefix.Uint16 | Prefix.LengthOnly)
            .WriteBytes(encrypted.AsSpan())
            .WriteByte(3);
        
        return new BinaryPacket()
            .WriteByte(2)
            .WriteUshort((ushort)(writer.Length + 2 + 1))
            .WritePacket(writer);
    }
    #endregion
}

public enum CredentialType
{
    EasyLogin,
    UnusualEasyLogin,
    PasswordLogin,
    UnusualPasswordLogin,
    NewDeviceLogin,
}

/// <summary>
/// State of QRCode Scanning
/// </summary>
public enum QrCodeState : byte
{
    Invalid = 0xFF,
    Confirmed = 0,
    CodeExpired = 17,
    WaitingForScan = 48,
    WaitingForConfirm = 53,
    Canceled = 54,
}