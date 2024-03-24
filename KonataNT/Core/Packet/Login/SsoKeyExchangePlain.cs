using KonataNT.Proto;

namespace KonataNT.Core.Packet.Login;

[ProtoContract]
internal partial class SsoKeyExchangePlain
{
    [ProtoMember(1)] public string? Uin { get; set; }
    
    [ProtoMember(2)] public byte[]? Guid { get; set; }
}