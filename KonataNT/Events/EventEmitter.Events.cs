using KonataNT.Events.EventArgs;

namespace KonataNT.Events;

public partial class EventEmitter
{
    public event KonataEvent<BotOnlineEvent>? OnBotOnlineEvent;

    public event KonataEvent<BotLogEvent>? OnBotLogEvent;
    
    public event KonataEvent<BotPrivateMessageEvent>? OnBotPrivateMessageEvent;
    
    public event KonataEvent<BotGroupMessageEvent>? OnBotGroupMessageEvent;
    
    public event KonataEvent<BotStrangerMessageEvent>? OnBotStrangerMessageEvent;
}