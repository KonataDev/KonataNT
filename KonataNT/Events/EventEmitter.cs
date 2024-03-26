using System.Runtime.CompilerServices;
using KonataNT.Core;
using KonataNT.Core.Handlers;
using KonataNT.Events.EventArgs;

namespace KonataNT.Events;

public partial class EventEmitter
{
    private const string Tag = "EventInvoker";
    
    private readonly Dictionary<Type, Action<EventBase>> _events = new();
    
    public delegate void KonataEvent<in TEvent>(BaseClient client, TEvent e) where TEvent : EventBase;

    public EventEmitter(BaseClient client)
    {
        RegisterEvent((BotOnlineEvent e) => OnBotOnlineEvent?.Invoke(client, e));
        RegisterEvent((BotLogEvent e) => OnBotLogEvent?.Invoke(client, e));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RegisterEvent<TEvent>(Action<TEvent> action) where TEvent : EventBase => _events[typeof(TEvent)] = e => action((TEvent)e);
    
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