using ProtoBuf;

namespace KonataNT.Core.Packet.System;

[ProtoContract]
internal class NTSsoHeartBeat
{
    [ProtoMember(1)] public int Type { get; set; }
}