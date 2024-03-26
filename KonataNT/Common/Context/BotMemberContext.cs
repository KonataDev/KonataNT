namespace KonataNT.Common.Context;

[Serializable]
public class BotMemberContext : IBotContactable
{
    internal BotMemberContext(BotClient client,
        uint uin, string uid,
        GroupMemberPermission permission, 
        uint groupLevel, string? memberCard, string memberName,
        DateTime joinTime, DateTime lastMsgTime)
    {
        Client = client;
        
        Uin = uin;
        Uid = uid;
        Permission = permission;
        GroupLevel = groupLevel;
        MemberCard = memberCard;
        MemberName = memberName;
        JoinTime = joinTime;
        LastMsgTime = lastMsgTime;
    }

    #region Properties

    public BotClient Client { get; }
    
    public uint Uin { get; set; }
    
    internal string Uid { get; set; }

    public GroupMemberPermission Permission { get; set; }
    
    public uint GroupLevel { get; set; }
    
    public string? MemberCard { get; set; }
    
    public string MemberName { get; set; }
    
    public DateTime JoinTime { get; set; }
    
    public DateTime LastMsgTime { get; set; }

    public string Avatar => $"https://q1.qlogo.cn/g?b=qq&nk={Uin}&s=640";

    #endregion
}

public enum GroupMemberPermission : uint
{
    Member = 0,
    Owner = 1,
    Admin = 2,
}