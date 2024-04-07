using ProtoBuf;

namespace KonataNT.Core.Packet.Oidb;

[ProtoContract]
internal class OidbNumber
{
    [ProtoMember(1)] public List<uint> Numbers { get; set; } = new();
}