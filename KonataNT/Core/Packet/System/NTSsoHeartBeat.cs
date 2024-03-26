using KonataNT.Proto;

namespace KonataNT.Core.Packet.System;

[ProtoContract]
internal partial class NTSsoHeartBeat
{
    [ProtoMember(1)] public int Type { get; set; }
}