using KonataNT.Common;
using KonataNT.Events;

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
}

public enum CredentialType
{
    EasyLogin,
    UnusualEasyLogin,
    PasswordLogin,
    UnusualPasswordLogin,
    NewDeviceLogin,
}