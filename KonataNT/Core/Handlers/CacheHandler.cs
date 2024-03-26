using KonataNT.Common.Context;

namespace KonataNT.Core.Handlers;

/// <summary>
/// Caching Uid, <see cref="BotFriendContext"/>, <see cref="BotGroupContext"/>, <see cref="BotMemberContext"/>
/// </summary>
internal class CacheHandler(BaseClient client)
{
    private Dictionary<uint, BotFriendContext> Friends { get; } = new();
    
    private Dictionary<uint, BotGroupContext> Groups { get; } = new();
    
    private Dictionary<uint, List<BotMemberContext>> Members { get; } = new();
    
    private Dictionary<string, uint> UidToUin { get; } = new();
    
    public async Task<uint> ResolveUin(uint? groupUin, string uid)
    {
        throw new NotImplementedException();
    }
    
    public async Task<string> ResolveUid(uint? groupUin, uint uin)
    {
        throw new NotImplementedException();
    }
    
    public async Task<BotFriendContext> GetFriend(uint uin, bool refreshCache = false)
    {
        if (refreshCache || Friends.Count == 0)
        {
            
        }
        
        throw new NotImplementedException();
    }
    
    public async Task<BotGroupContext> GetGroup(uint groupUin, bool refreshCache = false)
    {
        throw new NotImplementedException();
    }
    
    public async Task<List<BotMemberContext>> GetMembers(uint groupUin, bool refreshCache = false)
    {
        throw new NotImplementedException();
    }
}