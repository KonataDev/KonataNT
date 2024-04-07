using ProtoBuf;

namespace KonataNT.Core.Packet.Oidb;

#pragma warning disable CS8618

[ProtoContract]
internal class OidbFriendAdditional
{
    [ProtoMember(1)] public uint Type { get; set; }
    
    [ProtoMember(2)] public OidbFriendLayer1 Layer1 { get; set; }
}