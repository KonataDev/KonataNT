using ProtoBuf;

namespace KonataNT.Core.Packet.Login;

#pragma warning disable CS8618

[ProtoContract]
internal class SsoNTLoginBase
{
    [ProtoMember(1)] public SsoNTLoginHeader Header { get; set; }
    
    [ProtoMember(2)] public SsoNTLoginData Data { get; set; }
}