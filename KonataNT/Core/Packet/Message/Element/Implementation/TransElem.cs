using ProtoBuf;

// ReSharper disable InconsistentNaming
#pragma warning disable CS8618

namespace KonataNT.Core.Packet.Message.Element.Implementation;

[ProtoContract]
internal class TransElem
{
    [ProtoMember(1)] public int ElemType { get; set; }
    
    [ProtoMember(2)] public byte[] ElemValue { get; set; }
}