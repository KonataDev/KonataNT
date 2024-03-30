using ProtoBuf;

namespace KonataNT.Core.Packet.Message.Component.Extra;

[ProtoContract]
internal class PokeExtra
{
    [ProtoMember(1)] public uint Type { get; set; }
    
    [ProtoMember(7)] public uint Field7 { get; set; }
    
    [ProtoMember(8)] public uint Field8 { get; set; }
}