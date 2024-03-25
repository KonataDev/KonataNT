using System.Text;
using KonataNT.Common;
using KonataNT.Core.Packet;
using KonataNT.Events;
using KonataNT.Utility;
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
    
    public EventEmitter EventEmitter { get; init; }
    
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
        
        Logger.LogInformation(Tag, $"QR Code State: {state} | Uin: {KeyStore.Uin}");
        
        if (state == QrCodeState.Confirmed)
        {
            reader.Skip(4);
            KeyStore.Uin = reader.ReadUint();
            reader.Skip(4);
            
            var tlvBody = new TlvUnPacker(reader);
            KeyStore.TgtgtKey = tlvBody.TlvMap[0x1E];
            KeyStore.A2 = tlvBody.TlvMap[0x18];
            KeyStore.NoPicSig = tlvBody.TlvMap[0x19];
        }
        
        return state;
    }

    public async Task<bool> QrCodeLogin()
    {
        var tlv = new TlvPacker(KeyStore, AppInfo);
        var body = new BinaryPacket()
            .WriteUshort(0x09) // wtlogin_command
            .WritePacket(tlv.PackUp([0x106, 0x144, 0x116, 0x142, 0x145, 0x018, 0x141, 0x177, 0x191, 0x100, 0x107, 0x318, 0x16A, 0x166, 0x521], false));
        
        var login = BuildWtLoginPacket("wtlogin.login", body.ToArray());
        var response = await PacketHandler.SendPacket("wtlogin.login", login.ToArray());
        
        var decrypted = TeaProvider.Decrypt(response.AsSpan()[16..^1], KeyStore.ScepProvider.ShareKey);
        var reader = new BinaryPacket(decrypted);
        
        reader.Skip(2);
        ushort cmd = reader.ReadByte();
        var tlvResp = new TlvUnPacker(reader);

        if (tlvResp.TlvMap.TryGetValue(0x146, out var errorMsg) || tlvResp.TlvMap.TryGetValue(0x149, out errorMsg))
        {            
            var packet = new BinaryPacket(errorMsg);
            packet.Skip(2);
            string title = packet.ReadString(Prefix.Uint16 | Prefix.WithPrefix);
            string content = packet.ReadString(Prefix.Uint16 | Prefix.WithPrefix);
            
            Logger.LogError(Tag, $"Failed to login: {title} | {content}");
            return false;
        }

        if (tlvResp.TlvMap.TryGetValue(0x119, out var t119))
        { 
            t119 = TeaProvider.Decrypt(t119, KeyStore.TgtgtKey);
        }

        return true;
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

    private BinaryPacket BuildCode2dPacket(int cmd, byte[] buffer)
    {
        var packet = new BinaryPacket().WriteByte(0); // known const
        
        packet.Barrier(w =>
        {
            w.WriteUint((uint)AppInfo.AppId)
                .WriteUint(0x00000072) // const
                .WriteUshort(0) // const 0
                .WriteByte(0) // const 0
                .WriteUint((uint)DateTimeOffset.Now.ToUnixTimeSeconds()) // length actually starts here
                .WriteByte(0x02) // header for packet, counted into length of next barrier manually
                .Barrier(w => w
                    .WriteUshort((ushort)cmd)
                    .WriteUlong(0) // const 0
                    .WriteUint(0) // const 0
                    .WriteUlong(0) // const 0 
                    .WriteUshort(3) // const 3
                    .WriteUshort(0) // const 0
                    .WriteUshort(50) // unknown const
                    .WriteUlong(0)
                    .WriteUint(0)
                    .WriteUshort(0)
                    .WriteUint((uint)AppInfo.AppId)
                    .WriteBytes(buffer.AsSpan()), Prefix.Uint16 | Prefix.WithPrefix, 1); // addition is the packet start counted in

        }, Prefix.Uint16 | Prefix.WithPrefix, -13);

        return packet;
    }

    private BinaryPacket BuildWtLoginPacket(string cmd, byte[] buffer)
    {
        var encrypted = TeaProvider.Encrypt(buffer.AsSpan(), KeyStore.ScepProvider.ShareKey.AsSpan());
        
        var packet = new BinaryPacket()
            .WriteByte(2) // packet start
            .Barrier(w => w
                .WriteUshort(8001) // ver
                .WriteUshort((ushort)(cmd == "wtlogin.trans_emp" ? 2066 : 2064)) // cmd: wtlogin.trans_emp: 2066, wtlogin.login: 2064
                .WriteUshort(0) // unique wtLoginSequence for wtlogin packets only, should be stored in KeyStore
                .WriteUint(KeyStore.Uin) // uin, 0 for wtlogin.trans_emp
                .WriteByte(3) // extVer
                .WriteByte(135) // cmdVer
                .WriteUint(0) // actually unknown const 0
                .WriteByte(19) // pubId
                .WriteUshort(0) // insId
                .WriteUshort(AppInfo.AppClientVersion) // cliType
                .WriteUint(0) // retryTime
                .WriteByte(1) // const
                .WriteByte(1) // const
                .WriteBytes(new byte[16].AsSpan()) // randKey
                .WriteUshort(0x102) // unknown const, 腾讯你妈妈死啦
                .WriteBytes(KeyStore.ScepProvider.GetPublicKey(), Prefix.Uint16 | Prefix.LengthOnly) // pubKey
                .WriteBytes(encrypted.AsSpan())
                .WriteByte(3), Prefix.Uint16 | Prefix.WithPrefix, 1); // 0x03 is the packet end

        return packet;
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