using ProtoBuf;

#pragma warning disable CS8618

namespace KonataNT.Core.Packet.Oidb;

[ProtoContract]
internal class OidbFriend
{
    [ProtoMember(1)] public string Uid { get; set; }
    
    [ProtoMember(3)] public uint Uin { get; set; }
    
    [ProtoMember(10001)] public List<OidbFriendAdditional> Additional { get; set; }
}