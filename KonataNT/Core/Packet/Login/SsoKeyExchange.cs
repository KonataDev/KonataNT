using ProtoBuf;

#pragma warning disable CS8618

namespace KonataNT.Core.Packet.Login;

[ProtoContract]
internal class SsoKeyExchange
{
    [ProtoMember(1)] public byte[] PubKey { get; set; }
    
    [ProtoMember(2)] public int Type { get; set; }
    
    [ProtoMember(3)] public byte[] GcmCalc1 { get; set; }
    
    [ProtoMember(4)] public uint Timestamp { get; set; }
    
    [ProtoMember(5)] public byte[] GcmCalc2 { get; set; }
}