namespace KonataNT.Events;

public abstract class EventBase : System.EventArgs
{
    public DateTime EventTime { get; }

    public string EventMessage { get; protected set; }

    internal EventBase()
    {
        EventTime = DateTime.Now;
        EventMessage = "[Empty Event Message]";
    }

    public override string ToString() => $"[{EventTime:HH:mm:ss}] {EventMessage}";
}