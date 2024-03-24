using KonataNT.Common;
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
    
    internal ILogger Logger { get; init; }
    
    internal BaseClient(BotKeystore keystore, BotConfig config)
    {
        KeyStore = keystore;
        AppInfo = BotAppInfo.ProtocolToAppInfo[config.Protocol];
        Config = config;
        EventEmitter = new EventEmitter();
        Logger = config.Logger ?? new DefaultLogger(EventEmitter);
    }
    
    public async Task FetchQrCode()
    {
        
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
        .WriteUint(0x72)
        .WriteBytes(new byte[3])
        .WriteUint((uint)DateTimeOffset.Now.ToUnixTimeSeconds())
        .WriteByte(0x02)
        .WriteUshort((ushort)(49 + buffer.Length)) // actually this is manually calculated, real implementation is seen at MiraiGo
        .WriteUshort((ushort)cmd)
        .WriteBytes(new byte[21])
        .WriteByte(3)
        .WriteUint(50)
        .WriteBytes(new byte[14])
        .WriteUint(0)
        .WriteBytes(buffer);

    private BinaryPacket BuildWtLoginPacket(string cmd, byte[] buffer)
    {
        var encrypted = TeaProvider.Encrypt(buffer.AsSpan(), KeyStore.ScepProvider.ShareKey.AsSpan());
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
            .WriteUshort(0)
            .WriteUint(0)
            .WriteByte(1)
            .WriteByte(1)
            .WriteBytes(new byte[16])
            .WriteUshort(0x102)
            .WriteBytes(KeyStore.ScepProvider.GetPublicKey(), Prefix.Uint16 | Prefix.LengthOnly)
            .WriteBytes(encrypted.AsSpan())
            .WriteByte(3);
        
        return new BinaryPacket()
            .WriteByte(2)
            .WriteUshort((ushort)(writer.Length + 3))
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