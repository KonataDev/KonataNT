using KonataNT.Proto;

namespace KonataNT.Core.Packet.Login;

[ProtoContract]
internal partial class SsoNTLoginEncryptedData
{
    [ProtoMember(1)] public byte[]? Sign { get; set; }
    
    [ProtoMember(3)] public byte[]? GcmCalc { get; set; }
    
    [ProtoMember(4)] public int Type { get; set; }
}