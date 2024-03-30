using ProtoBuf;

#pragma warning disable CS8618
// ReSharper disable InconsistentNaming

namespace KonataNT.Core.Packet.Oidb;

[ProtoContract]
internal class OidbSvcTrpcTcp0x102A_1Response
{
    [ProtoMember(3)] public string ClientKey { get; set; }
}