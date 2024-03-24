using KonataNT.Proto;

namespace KonataNT.Core.Packet.Login;

[ProtoContract]
internal partial class SsoNTLoginCookie
{
    [ProtoMember(1)] public string? Cookie { get; set; }
}