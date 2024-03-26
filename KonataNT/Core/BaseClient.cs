using System.Security.Cryptography;
using System.Text;
using KonataNT.Common;
using KonataNT.Core.Handlers;
using KonataNT.Core.Packet;
using KonataNT.Core.Packet.Login;
using KonataNT.Core.Packet.Service;
using KonataNT.Core.Packet.System;
using KonataNT.Events;
using KonataNT.Events.EventArgs;
using KonataNT.Utility;
using KonataNT.Utility.Binary;
using KonataNT.Utility.Crypto;
using TaskScheduler = KonataNT.Utility.TaskScheduler;

namespace KonataNT.Core;

/// <summary>
/// The BaseClient that implements most basic login protocol.
/// </summary>
public class BaseClient : IDisposable
{
    private const string Tag = nameof(BaseClient);
    
    public string BotName => KeyStore.Info.Name;

    public uint BotUin => KeyStore.Uin;
    
    public EventEmitter EventEmitter { get; }
    
    public BotKeystore KeyStore { get; }
    
    internal BotAppInfo AppInfo { get; }
    
    internal BotConfig Config { get; }
    
    
    internal TaskScheduler Scheduler { get; }
    
    internal PacketHandler PacketHandler { get; }
    
    internal CacheHandler CacheHandler { get; }
    
    internal PushHandler PushHandler { get; }
    
    internal ILogger Logger { get; init; }
    
    internal BaseClient(BotKeystore keystore, BotConfig config)
    {
        KeyStore = keystore;
        AppInfo = BotAppInfo.ProtocolToAppInfo[config.Protocol];
        Config = config;
        Scheduler = new TaskScheduler();
        EventEmitter = new EventEmitter(this);
        PacketHandler = new PacketHandler(this);
        CacheHandler = new CacheHandler(this);
        PushHandler = new PushHandler(this);
        Logger = config.Logger ?? new DefaultLogger(EventEmitter);
    }
    
    public async Task<(string Url, byte[] Image)?> FetchQrCode()
    {
        if (KeyStore.D2.Length != 0)
        {
            if (DateTime.Now - KeyStore.SessionTime > TimeSpan.FromDays(15))
            {
                Logger.LogWarning(Tag, "Invalid Session, clearing session data.");
            
                KeyStore.D2 = Array.Empty<byte>();
                KeyStore.D2Key = new byte[16];
                KeyStore.Tgt = Array.Empty<byte>();
            }
            else
            {
                Logger.LogInformation(Tag, "Session is still valid, skipping QR code fetching.");
                Logger.LogInformation(Tag, "Trying to login with existing session.");
                await BotOnline();
                
                return null;
            }
        }
        
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
            var collection = new TlvUnPacker(new BinaryPacket(t119));

            KeyStore.A2 = collection.TlvMap[0x106];
            KeyStore.Tgt = collection.TlvMap[0x10a];
            KeyStore.D2 = collection.TlvMap[0x143];
            KeyStore.D2Key = collection.TlvMap[0x305];

            var raw = collection.TlvMap[0x543];
            var layer = Tlv543.Deserialize(raw);
            var layer1 = ((Tlv543)layer).Layer1;
            var layer2 = layer1.Layer2;
            KeyStore.Uid = layer2.Uid;
            
            var uidRaw = collection.TlvMap[0x543];
            var t11A = collection.TlvMap[0x11a];

            KeyStore.Info = new BotInfo
            {
                Name = Encoding.UTF8.GetString(uidRaw[5..]),
                Age = t11A[2],
                Gender = t11A[3]
            };
            
            await BotOnline();
            Logger.LogInformation(Tag, "Login Success!");
        }

        return true;
    }

    public async Task Login()
    {
        if (KeyStore is not { ExchangeKey: not null, KeySign: not null })
        {
            Logger.LogFatal(Tag, "Please exchange key first.");
            return;
            
            
        }
    }
    
    /// <summary>
    /// Called when Bot is online through login, or reconnected / session resumed.
    /// </summary>
    private async Task BotOnline(bool isReconnect = false)
    {
        var statusRegister = new StatusRegister
        {
            Guid = KeyStore.Guid.Hex(),
            Type = 0,
            CurrentVersion = AppInfo.CurrentVersion,
            Field4 = 0,
            LocaleId = 2052,
            Online = new OnlineOsInfo
            {
                User = KeyStore.Name,
                Os = AppInfo.Kernel,
                OsVer = "Linux",
                VendorName = "",
                OsLower = AppInfo.VendorOs,
            },
            SetMute = 0,
            RegisterVendorType = 0,
            RegType = 1,
        };

        const string command = "trpc.qq_new_tech.status_svc.StatusService.Register";
        var resp = await PacketHandler.SendPacket(command, statusRegister.Serialize());

        if (StatusRegisterResponse.Deserialize(resp) is StatusRegisterResponse { Message: not null } response)
        {
            KeyStore.SessionTime = DateTime.Now;
            Logger.LogInformation(Tag, $"Status Register Response: {response.Message}");
            
            if (response.Message.Contains("success"))
            {
                Scheduler.Interval("SsoHeartBeat", response.Field4 * 1000, async () =>
                {
                    var packet = BuildSsoHeartBeatPacket();
                    await PacketHandler.SendPacket("trpc.qq_new_tech.status_svc.StatusService.SsoHeartBeat", packet);
                });
                
                var arg = new BotOnlineEvent(isReconnect
                    ? BotOnlineEvent.OnlineReason.Reconnect
                    : BotOnlineEvent.OnlineReason.Login);
                EventEmitter.PostEvent(arg);
            }
        }
    }

    #region Private Builders

    private byte[] BuildSsoHeartBeatPacket() => new NTSsoHeartBeat { Type = 1 }.Serialize();

    private byte[] BuildKeyExchangePacket()
    {
        const string gcmCalc2Key = "e2733bf403149913cbf80c7a95168bd4ca6935ee53cd39764beebe2e007e3aee";
        uint timestamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var shareKey = KeyStore.PrimeProvider.ShareKey;

        var plain = new SsoKeyExchangePlain
        {
            Uin = KeyStore.Uin.ToString(),
            Guid = KeyStore.Guid
        }.Serialize();
        
        var hashPlain = new BinaryPacket()
            .WriteBytes(KeyStore.PrimeProvider.GetPublicKey(false))
            .WriteUint(1)
            .WriteBytes(plain)
            .WriteUint(0)
            .WriteUint(timestamp);
        
        var hash = SHA256.HashData(hashPlain.ToArray());

        var gcmClac1 = AesProvider.Encrypt(plain, shareKey);
        var gcmClac2 = AesProvider.Encrypt(hash, gcmCalc2Key.UnHex());

        return new SsoKeyExchange
        {
            PubKey = KeyStore.PrimeProvider.GetPublicKey(false),
            Type = 1,
            GcmCalc1 = gcmClac1,
            Timestamp = timestamp,
            GcmCalc2 = gcmClac2
        }.Serialize();
    }

    private byte[] BuildNTLoginPacket(byte[] credential)
    {
        if (KeyStore.ExchangeKey is null || KeyStore.KeySign is null) throw new InvalidOperationException("Key is null");
        
        var header = new SsoNTLoginHeader
        {
            Uin = new SsoNTLoginUin
            {
                Uid = KeyStore.Uid
            },
            System = new SsoNTLoginSystem
            {
                Os = AppInfo.Os,
                DeviceName = KeyStore.Name,
                Type = 7,
                Guid = KeyStore.Guid
            },
            Version = new SsoNTLoginVersion
            {
                KernelVersion = "",
                AppId = AppInfo.AppId,
                PackageName = AppInfo.PackageName
            },
            Cookie = new SsoNTLoginCookie
            {
                Cookie = KeyStore.SessionCookie
            }
        };
        
        var packet = new SsoNTLoginBase
        {
            Header = header,
            Data = new SsoNTLoginData
            {
                Credential = credential
            }
        };
        
        if (KeyStore.Captcha is { Aid: not null, Ticket: not null, RandStr: not null })
        {
            packet.Data.Captcha = new SsoNTLoginCaptcha
            {
                Aid = KeyStore.Captcha.Aid,
                Ticket = KeyStore.Captcha.Ticket,
                RandStr = KeyStore.Captcha.RandStr
            };
        }

        var encrypted = new SsoNTLoginEncryptedData
        {
            Sign = KeyStore.KeySign,
            GcmCalc = AesProvider.Encrypt(packet.Serialize(), KeyStore.ExchangeKey),
            Type = 1
        };
        return encrypted.Serialize();
    }

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

    public void Dispose()
    {
        Scheduler.Dispose();
    }
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