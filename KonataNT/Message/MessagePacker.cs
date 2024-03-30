using KonataNT.Core.Packet.Message;

namespace KonataNT.Message;

internal static class MessagePacker
{
    public static MessageStruct Parse(PushMsgBody msg)
    {
        string name = msg.ResponseHead.Grp?.MemberCard ?? "";
        uint timestamp = msg.ContentHead.Timestamp;
        var @struct = new MessageStruct(msg.ResponseHead.FromUin, name, DateTime.Now)
        {
            Time = DateTime.UnixEpoch.AddSeconds(timestamp),
            Sequence = msg.ContentHead.Sequence
        };

        return @struct;
    }

    public static MessageChain Build(BotClient client, MessageStruct @struct)
    {
        throw new NotImplementedException();
    }
}