using ProtoBuf;

namespace KonataNT.Core.Packet.Login;

[ProtoContract]
internal class SsoNTLoginData
{
    [ProtoMember(1)] public byte[]? Credential { get; set; }
    
    [ProtoMember(2)] public SsoNTLoginCaptcha? Captcha { get; set; }
}