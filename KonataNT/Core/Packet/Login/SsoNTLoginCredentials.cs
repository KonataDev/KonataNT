using ProtoBuf;

#pragma warning disable CS8618

namespace KonataNT.Core.Packet.Login;

[ProtoContract]
internal class SsoNTLoginCredentials
{
    [ProtoMember(3)] public byte[] TempPassword { get; set; }
    
    [ProtoMember(4)] public byte[] Tgt { get; set; }

    [ProtoMember(5)] public byte[] D2 { get; set; }
    
    [ProtoMember(6)] public byte[] D2Key { get; set; }
}