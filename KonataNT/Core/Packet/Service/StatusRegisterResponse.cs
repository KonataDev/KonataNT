using KonataNT.Proto;

namespace KonataNT.Core.Packet.Service;

[ProtoContract]
internal partial class StatusRegisterResponse
{
    [ProtoMember(2)] public string? Message { get; set; }
    
    [ProtoMember(3)] public uint Timestamp { get; set; }
    
    [ProtoMember(4)] public int Field4 { get; set; }
    
    [ProtoMember(5)] public int Field5 { get; set; }
}