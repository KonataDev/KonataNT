namespace KonataNT.Common.Context;

[Serializable]
public class BotGroupContext : IBotContactable
{
    internal BotGroupContext(BotClient client, uint groupUin, string groupName, uint memberCount, uint maxMember)
    {
        Client = client;
        
        GroupUin = groupUin;
        GroupName = groupName;
        MemberCount = memberCount;
        MaxMember = maxMember;
    }
    
    public BotClient Client { get; }
    
    public uint GroupUin { get; }
    
    public string GroupName { get; }
    
    public uint MemberCount { get; }
    
    public uint MaxMember { get; }

    public string Avatar => $"https://p.qlogo.cn/gh/{GroupUin}/{GroupUin}/0/";
}