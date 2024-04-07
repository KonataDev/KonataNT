using ProtoBuf;

namespace KonataNT.Core.Packet.Oidb;

#pragma warning disable CS8618

[ProtoContract]
internal class OidbFriendLayer1
{
    [ProtoMember(2)] public List<OidbFriendProperty> Properties { get; set; }
}