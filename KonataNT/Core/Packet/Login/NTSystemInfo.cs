using KonataNT.Proto;

namespace KonataNT.Core.Packet.Login;

#pragma warning disable CS8618

[ProtoContract]
internal partial class NTSystemInfo
{
    [ProtoMember(1)] public string Os { get; set; }

    [ProtoMember(2)] public string DeviceName { get; set; }
}

[ProtoContract]
internal partial class NTLoginInfo
{
    [ProtoMember(1)] public NTSystemInfo SystemInfo { get; set; }
    
    [ProtoMember(2)] public string? QrCode { get; set; }
    
    [ProtoMember(3)] public string? QrSig { get; set; }
    
    [ProtoMember(4)] public byte[] Type { get; set; }
}