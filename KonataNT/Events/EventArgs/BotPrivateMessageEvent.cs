using KonataNT.Message;

namespace KonataNT.Events.EventArgs;

public class BotPrivateMessageEvent : EventBase
{
    public uint FriendUin { get; }
    
    public MessageStruct Message { get; }
    
    public BotPrivateMessageEvent(uint friendUin, MessageStruct message)
    {
        FriendUin = friendUin;
        Message = message;
        
        EventMessage = $"[{nameof(BotPrivateMessageEvent)}] {friendUin} {message}";
    }
}