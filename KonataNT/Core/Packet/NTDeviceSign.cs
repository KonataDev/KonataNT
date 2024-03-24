using KonataNT.Proto;

namespace KonataNT.Core.Packet;

[ProtoContract]
internal partial class NTDeviceSign
{
    [ProtoMember(15)] public string? Trace { get; set; }
    
    [ProtoMember(16)] public string? Uid { get; set; }
    
    [ProtoMember(24)] public Sign? Sign { get; set; }
}

[ProtoContract]
internal partial class Sign
{
    [ProtoMember(1)] public byte[]? Signature { get; set; }
    
    [ProtoMember(2)] public string? Token { get; set; }
    
    [ProtoMember(3)] public byte[]? Extra { get; set; }
}