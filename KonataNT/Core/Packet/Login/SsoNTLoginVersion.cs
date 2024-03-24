using KonataNT.Proto;

namespace KonataNT.Core.Packet.Login;

[ProtoContract]
internal partial class SsoNTLoginVersion
{
    [ProtoMember(1)] public string? KernelVersion { get; set; }

    [ProtoMember(2)] public int AppId { get; set; }

    [ProtoMember(3)] public string? PackageName { get; set; }
}