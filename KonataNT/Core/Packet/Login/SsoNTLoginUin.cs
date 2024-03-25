using KonataNT.Proto;

namespace KonataNT.Core.Packet.Login;

[ProtoContract]
internal partial class SsoNTLoginUin
{
    [ProtoMember(1)] public string? Uid { get; set; } = string.Empty;
}