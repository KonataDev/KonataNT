using ProtoBuf;

namespace KonataNT.Core.Packet.Message.Component.Extra;

[ProtoContract]
internal class FileExtra
{
    [ProtoMember(1)] public NotOnlineFile? File { get; set; }
}