using ProtoBuf;

namespace KonataNT.Core.Packet.Login;

[ProtoContract]
internal class SsoNTLoginHeader
{
    [ProtoMember(1)] public SsoNTLoginUin? Uin { get; set; }
    
    [ProtoMember(2)] public SsoNTLoginSystem? System { get; set; }
    
    [ProtoMember(3)] public SsoNTLoginVersion? Version { get; set; }
    
    [ProtoMember(4)] public SsoNTLoginError? Error { get; set; }
    
    [ProtoMember(5)] public SsoNTLoginCookie? Cookie { get; set; }
}