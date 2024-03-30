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
    
    private Dictionary<uint, string> UinToUid { get; } = new();

    public string this[uint index] => UinToUid[index];
    
    public uint this[string index] => UinToUid.FirstOrDefault(x => x.Value == index).Key;
    
    public async Task<BotFriendContext> GetFriend(uint uin, bool refreshCache = false)
    {
        if (refreshCache || Friends.Count == 0)  // count == 0 for initial cache
        {
            
        }
        
        throw new NotImplementedException("干什么！");
    }
    
    public async Task<BotGroupContext> GetGroup(uint groupUin, bool refreshCache = false)
    {
        if (refreshCache || Groups.Count == 0)  // count == 0 for initial cache
        {
            
        }
        
        throw new NotImplementedException("干什么！");
    }
    
    public async Task<List<BotMemberContext>> GetMembers(uint groupUin, bool refreshCache = false)
    {
        if (refreshCache || !Members.TryGetValue(groupUin, out var members))
        {
            
        }

        if (Members.TryGetValue(groupUin, out members)) return members;
        
        throw new NotImplementedException("干什么！");
    }
}