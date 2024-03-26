namespace KonataNT.Message;

[Serializable]
public class MessageResult
{
    public uint Sequence { get; set; }
    
    public ulong MessageId { get; set; } // actually calculated by message random
    
    public uint Result { get; set; }
}