namespace KonataNT.Events;

public partial class EventEmitter
{
    private const string Tag = "EventInvoker";
    
    private readonly Dictionary<Type, Action<EventBase>> _events;
    
    public delegate void KonataEvent<in TEvent>(BotClient client, TEvent e) where TEvent : EventBase;
}