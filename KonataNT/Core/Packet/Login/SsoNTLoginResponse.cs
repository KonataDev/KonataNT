using ProtoBuf;

namespace KonataNT.Core.Packet.Login;

[ProtoContract]
internal class SsoNTLoginResponse
{
    [ProtoMember(1)] public SsoNTLoginCredentials? Credentials { get; set; }
}