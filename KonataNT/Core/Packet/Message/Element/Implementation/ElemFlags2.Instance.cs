using ProtoBuf;

namespace KonataNT.Core.Packet.Message.Element.Implementation;

internal partial class ElemFlags2
{
    [ProtoContract]
    public class Instance
    {
        [ProtoMember(1)] public uint AppId { get; set; }
        
        [ProtoMember(2)] public uint InstId { get; set; }
    }
}