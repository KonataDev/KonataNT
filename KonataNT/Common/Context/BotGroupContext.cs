using KonataNT.Message;

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

    #region Properties

    public BotClient Client { get; }

    public uint GroupUin { get; }
    
    public string GroupName { get; }
    
    public uint MemberCount { get; }
    
    public uint MaxMember { get; }

    public string Avatar => $"https://p.qlogo.cn/gh/{GroupUin}/{GroupUin}/0/";

    #endregion

    public async Task<bool> LeaveGroup()
    {
        throw new NotImplementedException();
    }
    
    public async Task<bool> SetName(string groupName)
    {
        throw new NotImplementedException();
    }
    
    public async Task<bool> SendRemark(string remark)
    {
        throw new NotImplementedException();
    }
    
    public async Task<bool> SetMute(bool mute)
    {
        throw new NotImplementedException();
    }
    
    public Task<MessageResult> SendMessage()
    {
        throw new NotImplementedException();
    }
    
    public async Task<bool> Invite(uint targetGroup)
    {
        throw new NotImplementedException();
    }
    
    public Task<BotMemberContext> FetchMembers(bool refreshCache = false)
    {
        throw new NotImplementedException();
    }
}