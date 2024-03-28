using ProtoBuf;

namespace KonataNT.Core.Packet.Login;

[ProtoContract]
internal class SsoNTLoginUin
{
    [ProtoMember(1)] public string? Uin { get; set; } = string.Empty;
}