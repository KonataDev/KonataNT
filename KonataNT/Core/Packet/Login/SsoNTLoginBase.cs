using ProtoBuf;

namespace KonataNT.Core.Packet.Login;

#pragma warning disable CS8618

[ProtoContract]
internal class SsoNTLoginBase<T>
{
    [ProtoMember(1)] public SsoNTLoginHeader Header { get; set; }
    
    [ProtoMember(2)] public T Data { get; set; }
}