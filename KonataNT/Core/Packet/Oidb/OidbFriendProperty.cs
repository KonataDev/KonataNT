using ProtoBuf;

namespace KonataNT.Core.Packet.Oidb;

#pragma warning disable CS8618

[ProtoContract]
internal class OidbFriendProperty
{
    [ProtoMember(1)] public uint Code { get; set; }
    
    [ProtoMember(2)] public string Value { get; set; }
}