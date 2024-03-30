using ProtoBuf;

namespace KonataNT.Core.Packet.Message.Element.Implementation.Extra;

[ProtoContract]
internal class CustomFaceExtra
{
    [ProtoMember(31)] public string? Hash { get; set; }
}