using ProtoBuf;

#pragma warning disable CS8618

namespace KonataNT.Core.Packet.Message.Component.Extra;

[ProtoContract]
internal class ImageExtraUrl
{
    [ProtoMember(30)] public string OrigUrl { get; set; }
}
