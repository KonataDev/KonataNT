using KonataNT.Common.Context;
using KonataNT.Core.Packet.Oidb;
using KonataNT.Core.Packet.Service.Oidb;
using KonataNT.Utility;

namespace KonataNT.Core.Handlers;

/// <summary>
/// Caching Uid, <see cref="BotFriendContext"/>, <see cref="BotGroupContext"/>, <see cref="BotMemberContext"/>
/// </summary>
internal class CacheHandler
{
    private readonly BaseClient _client;
    

    /// <summary>
    /// Caching Uid, <see cref="BotFriendContext"/>, <see cref="BotGroupContext"/>, <see cref="BotMemberContext"/>
    /// </summary>
    public CacheHandler(BaseClient client)
    {
        _client = client;

        _client.EventEmitter.OnBotGroupMessageEvent += async (_, e) =>
        {
            if (!Members.ContainsKey(e.GroupUin)) await GetMembers(e.GroupUin);
        };
    }

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
            var memberList = new List<BotMemberContext>();
            string? token = null;
            
            while (token != null)
            {
                var packet = new OidbSvcTrpcTcp0xFE7_3
                {
                    GroupUin = groupUin,
                    Field2 = 5,
                    Field3 = 2,
                    Body = new OidbSvcTrpcScp0xFE7_3Body
                    {
                        MemberName = true,
                        MemberCard = true,
                        Level = true,
                        JoinTimestamp = true,
                        LastMsgTimestamp = true,
                        Permission = true,
                    },
                    Token = token
                };
                
                var response = await _client.PacketHandler.SendOidb(0xfe7, 3, packet.Serialize(), false);
                var payload = response.Deserialize<OidbSvcBase>();
                var body = payload.Body?.Deserialize<OidbSvcTrpcTcp0xFE7_2Response>();
                
                if (body == null) break;

                token = body.Token;
            }
        }

        if (Members.TryGetValue(groupUin, out members)) return members;
        
        throw new NotImplementedException("干什么！");
    }
}