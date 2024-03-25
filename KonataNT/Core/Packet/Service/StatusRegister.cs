using KonataNT.Proto;

namespace KonataNT.Core.Packet.Service;

[ProtoContract]
internal partial class StatusRegister
{
    [ProtoMember(1)] public string? Guid { get; set; }

    [ProtoMember(2)] public int? Type { get; set; }

    [ProtoMember(3)] public string? CurrentVersion { get; set; }

    [ProtoMember(4)] public int? Field4 { get; set; }

    [ProtoMember(5)] public int? LocaleId { get; set; } // 2052

    [ProtoMember(6)] public OnlineOsInfo? Online { get; set; }

    [ProtoMember(7)] public int? SetMute { get; set; }

    [ProtoMember(8)] public int? RegisterVendorType { get; set; }

    [ProtoMember(9)] public int? RegType { get; set; }
}

[ProtoContract]
internal partial class OnlineOsInfo
{
    [ProtoMember(1)] public string? User { get; set; }
	
    [ProtoMember(2)] public string? Os { get; set; }
	
    [ProtoMember(3)] public string? OsVer { get; set; }
	
    [ProtoMember(4)] public string? VendorName { get; set; }
	
    [ProtoMember(5)] public string? OsLower { get; set; }
}