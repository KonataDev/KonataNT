using ProtoBuf;

// ReSharper disable InconsistentNaming
#pragma warning disable CS8618

namespace KonataNT.Core.Packet.Message.Element.Implementation;

[ProtoContract]
internal partial class QQWalletMsg
{
    [ProtoMember(1)] public QQWalletAioBody Type { get; set; }
}