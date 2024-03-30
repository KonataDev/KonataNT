using KonataNT.Core.Packet.Message;
using KonataNT.Events;
using KonataNT.Events.EventArgs;

namespace KonataNT.Message;

internal static class MessagePacker
{
    public static EventBase Parse(PushMsgBody msg)
    {
        string name = msg.ResponseHead.Grp?.MemberCard ?? "";
        uint timestamp = msg.ContentHead.Timestamp;
        var @struct = new MessageStruct(msg.ResponseHead.FromUin, name, DateTime.Now)
        {
            Time = DateTime.UnixEpoch.AddSeconds(timestamp),
            Sequence = msg.ContentHead.Sequence,
            SourceType = msg.ResponseHead.Grp is not null ? MessageStruct.Source.Group : MessageStruct.Source.Friend
        };

        return @struct.SourceType switch
        {
            MessageStruct.Source.Group => new BotGroupMessageEvent(msg.ResponseHead.Grp?.GroupUin ?? 0, msg.ResponseHead.FromUin, @struct),
            MessageStruct.Source.Friend => new BotPrivateMessageEvent(msg.ResponseHead.FromUin, @struct),
            _ => throw new NotImplementedException()
        };
    }

    public static MessageChain Build(BotClient client, MessageStruct @struct)
    {
        throw new NotImplementedException();
    }
}