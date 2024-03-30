using KonataNT.Message;

namespace KonataNT.Events.EventArgs;

public class BotGroupMessageEvent : EventBase
{
    public BotGroupMessageEvent(uint groupUin, uint memberUin, MessageStruct message)
    {
        GroupUin = groupUin;
        MemberUin = memberUin;
        Message = message;
        
        EventMessage = $"[{nameof(BotGroupMessageEvent)}] {groupUin} {memberUin} {message}";
    }

    public uint GroupUin { get; }
    
    public uint MemberUin { get; }
    
    public MessageStruct Message { get; }
}