using ProtoBuf;

namespace KonataNT.Core.Packet.Login;

[ProtoContract]
internal class SsoNTLoginCookie
{
    [ProtoMember(1)] public string? Cookie { get; set; }
}