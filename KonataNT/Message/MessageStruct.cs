namespace KonataNT.Message;

public class MessageStruct
{
    public DateTime Time { get; internal set; }

    public uint Sequence { get; internal set; }

    public MessageChain Chain { get; internal set; }

    public enum Source
    {
        Group,
        Friend,
        Stranger
    }
    
    /// <summary>
    /// <b>[In] [Out]</b>     <br/>
    /// Sender info
    /// </summary>
    public (uint Uin, string Name) Sender { get; internal set; }

    /// <summary>
    /// Construct fake source info
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="name"></param>
    /// <param name="messageTime"></param>
    public MessageStruct(uint uin, string name, DateTime messageTime)
    {
        Sender = (uin, name);
        Time = messageTime.ToUniversalTime();
        Chain = new MessageChain();
    }
}
