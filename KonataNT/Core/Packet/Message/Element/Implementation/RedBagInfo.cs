using ProtoBuf;

namespace KonataNT.Core.Packet.Message.Element.Implementation;

[ProtoContract]
internal class RedBagInfo
{
    [ProtoMember(1)] public uint? RedBagType { get; set; }
}