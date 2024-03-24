namespace KonataNT.Core;

/// <summary>
/// The BaseClient that implements most basic login protocol.
/// </summary>
public class BaseClient
{
    public async Task FetchQrCode()
    {
        
    }
    
    public async Task QrCodeLogin()
    {
        
    }

    public async Task Login(Memory<byte> credentials, CredentialType type)
    {
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

    internal async Task SendPacket(Memory<byte> payload, string command)
    {
        
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