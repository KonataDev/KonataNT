using KonataNT.Proto;

namespace KonataNT.Core.Packet.Login;

[ProtoContract]
internal partial class SsoNTLoginCaptcha
{
    [ProtoMember(1)] public string? Ticket { get; set; }
    
    [ProtoMember(2)] public string? RandStr { get; set; }
    
    [ProtoMember(3)] public string? Aid { get; set; }
}