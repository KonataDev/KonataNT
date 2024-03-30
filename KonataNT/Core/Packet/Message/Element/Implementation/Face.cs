using ProtoBuf;

namespace KonataNT.Core.Packet.Message.Element.Implementation;

[ProtoContract]
internal class Face
{
    [ProtoMember(1)] public int? Index { get; set; }
    
    [ProtoMember(2)] public byte[]? Old { get; set; }
    
    [ProtoMember(11)] public byte[]? Buf { get; set; }
}