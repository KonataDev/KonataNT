using ProtoBuf;

#pragma warning disable CS8618

namespace KonataNT.Core.Packet.Message;

[ProtoContract]
internal class PushMsg
{
    [ProtoMember(1)] public PushMsgBody Message { get; set; }
}

[ProtoContract]
internal class PushMsgBody
{
    [ProtoMember(1)] public ResponseHead ResponseHead { get; set; }
    
    [ProtoMember(2)] public ContentHead ContentHead { get; set; }
}

[ProtoContract]
internal class ResponseHead
{
    [ProtoMember(1)] public uint FromUin { get; set; }
    
    [ProtoMember(2)] public string? FromUid { get; set; }
    
    [ProtoMember(3)] public uint Type { get; set; }
    
    [ProtoMember(4)] public uint SigMap { get; set; } // 鬼知道是啥
    
    [ProtoMember(5)] public uint ToUin { get; set; }
    
    [ProtoMember(6)] public string? ToUid { get; set; }
    
    [ProtoMember(8)] public GrpRouting? Grp { get; set; }
    
    
}

[ProtoContract]
internal class GrpRouting
{
    [ProtoMember(1)] public uint GroupUin { get; set; }
    
    [ProtoMember(2)] public uint Field2 { get; set; }
    
    [ProtoMember(3)] public uint Field3 { get; set; }
    
    [ProtoMember(4)] public string MemberCard { get; set; }
    
    [ProtoMember(5)] public uint Field4 { get; set; }
    
    [ProtoMember(6)] public uint Field5 { get; set; }
    
    [ProtoMember(7)] public string GroupName { get; set; }
    
    [ProtoMember(9)] public uint GroupFlag { get; set; }
}

[ProtoContract]
internal class ContentHead
{
    [ProtoMember(1)] public uint Type { get; set; }
    
    [ProtoMember(5)] public uint Sequence { get; set; }
    
    [ProtoMember(6)] public uint Timestamp { get; set; }
}