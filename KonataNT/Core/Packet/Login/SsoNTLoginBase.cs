using KonataNT.Proto;

namespace KonataNT.Core.Packet.Login;

#pragma warning disable CS8618

[ProtoContract]
internal partial class SsoNTLoginBase
{
    [ProtoMember(1)] public SsoNTLoginHeader Header { get; set; }
    
    [ProtoMember(2)] public byte[] Body { get; set; }
}