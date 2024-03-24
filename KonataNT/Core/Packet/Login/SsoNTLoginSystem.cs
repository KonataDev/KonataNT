using KonataNT.Proto;

namespace KonataNT.Core.Packet.Login;

[ProtoContract]
internal partial class SsoNTLoginSystem
{
    [ProtoMember(1)] public string Os { get; set; } = string.Empty;
    
    [ProtoMember(2)] public string DeviceName { get; set; } = string.Empty;

    [ProtoMember(3)] public int Type { get; set; } = 7; // ?

    [ProtoMember(4)] public byte[] Guid { get; set; } = Array.Empty<byte>();
}