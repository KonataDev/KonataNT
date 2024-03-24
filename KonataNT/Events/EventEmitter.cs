using KonataNT.Core;
using KonataNT.Events.EventArgs;

namespace KonataNT.Events;

public partial class EventEmitter
{
    private const string Tag = "EventInvoker";
    
    private readonly Dictionary<Type, Action<EventBase>> _events = new();
    
    public delegate void KonataEvent<in TEvent>(BotClient client, TEvent e) where TEvent : EventBase;

    public void RegisterEvent()
    {
        
    }
    
    internal void PostEvent(EventBase e)
    {
        Task.Run(() =>
        {
            try
            {
                if (_events.TryGetValue(e.GetType(), out var action)) action(e);
                else PostEvent(new BotLogEvent(Tag, LogLevel.Warning, $"Event {e.GetType().Name} is not registered but pushed to invoker"));
            }
            catch (Exception ex)
            {
                PostEvent(new BotLogEvent(Tag, LogLevel.Error, $"{ex.StackTrace}\n{ex.Message}"));
            }
        });
    }
}