using ProtoBuf;

namespace KonataNT.Core.Packet.Service.Oidb;

[ProtoContract]
internal class OidbSvcBase
{
    [ProtoMember(1)] public ushort Command { get; set; }
    
    [ProtoMember(2)] public ushort SubCommand { get; set; }
    
    [ProtoMember(3)] public int ErrorCode { get; set; }
    
    [ProtoMember(4)] public byte[]? Body { get; set; }
    
    [ProtoMember(5)] public string? ErrorMsg { get; set; }
    
    [ProtoMember(12)] public bool IsUid { get; set; }
}