using ProtoBuf;

#pragma warning disable CS8618
// Resharper disable InconsistentNaming

namespace KonataNT.Core.Packet.Oidb;

[ProtoContract]
internal class OidbSvcTrpcTcp0xFE7_3
{
    [ProtoMember(1)] public uint GroupUin { get; set; }
    
    [ProtoMember(2)] public uint Field2 { get; set; } // 5
    
    [ProtoMember(3)] public uint Field3 { get; set; } // 2
    
    [ProtoMember(4)] public OidbSvcTrpcScp0xFE7_3Body Body { get; set; }
    
    [ProtoMember(15)] public string? Token { get; set; }
}

[ProtoContract]
internal class OidbSvcTrpcScp0xFE7_3Body
{
    [ProtoMember(10)] public bool MemberName { get; set; } // 1
    
    [ProtoMember(11)] public bool MemberCard { get; set; } // 1
    
    [ProtoMember(12)] public bool Level { get; set; } // 1
    
    [ProtoMember(20)] public bool Field4 { get; set; } // 1
    
    [ProtoMember(21)] public bool Field5 { get; set; } // 1
    
    [ProtoMember(100)] public bool JoinTimestamp { get; set; } // 1
    
    [ProtoMember(101)] public bool LastMsgTimestamp { get; set; } // 1
    
    [ProtoMember(102)] public bool Field8 { get; set; } // 1
    
    [ProtoMember(103)] public bool Field9 { get; set; } // 1
    
    [ProtoMember(104)] public bool Field10 { get; set; } // 1
    
    [ProtoMember(105)] public bool Field11 { get; set; } // 1
    
    [ProtoMember(106)] public bool Field12 { get; set; } // 1
    
    [ProtoMember(107)] public bool Permission { get; set; } // 1
}