using KonataNT.Utility;

namespace KonataNT.Message;

public class MessageStruct
{
    public DateTime Time { get; private set; }

    public uint Sequence { get; private set; }

    public MessageChain Chain { get; private set; }

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
    public (uint Uin, string Name) Sender { get; private set; }

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
    }
}
