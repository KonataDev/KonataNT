using KonataNT.Proto;

namespace KonataNT.Core.Packet.Login;

[ProtoContract]
internal partial class SsoNTLoginError
{
    [ProtoMember(1)] public uint ErrorCode { get; set; }
    
    [ProtoMember(2)] public string? Tag { get; set; } = string.Empty;

    [ProtoMember(3)] public string? Message { get; set; } = string.Empty;
    
    [ProtoMember(5)] public string? NewDeviceVerifyUrl { get; set; } = string.Empty;
}