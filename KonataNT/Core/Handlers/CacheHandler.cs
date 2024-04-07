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
            if (!_groups.ContainsKey(e.GroupUin)) await GetGroups(e.GroupUin);
            if (!_members.ContainsKey(e.GroupUin)) await GetMembers(e.GroupUin);
        };
        
        _client.EventEmitter.OnBotPrivateMessageEvent += async (_, e) =>
        {
            if (!_friends.ContainsKey(e.FriendUin)) await GetFriend(e.FriendUin);
        };
    }

    private readonly Dictionary<uint, BotFriendContext> _friends = new();
    
    private readonly Dictionary<uint, BotGroupContext> _groups = new();
    
    private readonly Dictionary<uint, List<BotMemberContext>> _members = new();
    
    private readonly Dictionary<uint, string> _uinToUid = new();

    public string this[uint index] => _uinToUid[index];
    
    public uint this[string index] => _uinToUid.FirstOrDefault(x => x.Value == index).Key;
    
    public async Task<BotFriendContext> GetFriend(uint uin, bool refreshCache = false)
    {
        if (refreshCache || _friends.Count == 0)  // count == 0 for initial cache
        {
            var packet = new OidbSvcTrpcTcp0xFD4_1
            {
                Body =
                [
                    new OidbSvcTrpcTcp0xFD4_1Body { Type = 1, Number = new OidbNumber { Numbers = { 103, 102, 20002 } } },
                    new OidbSvcTrpcTcp0xFD4_1Body { Type = 4, Number = new OidbNumber { Numbers = { 100, 101, 102 } } }
                ]
            };
            
            var response = await _client.PacketHandler.SendOidb(0xfd4, 1, packet.Serialize(), false);
            var payload = response.Deserialize<OidbSvcBase>();
            var body = payload.Body?.Deserialize<OidbSvcTrpcTcp0xFD4_1Response>();
            
            if (body == null) throw new InvalidOperationException("干什么！");
            
            foreach (var raw in body.Friends)
            {
                var additional = raw.Additional.First(x => x.Type == 1);
                var properties = additional.Layer1.Properties.ToDictionary(x => x.Code, x => x.Value);
                _friends[raw.Uin] = new BotFriendContext((BotClient)_client, raw.Uin, raw.Uid, properties[20002], properties[103], properties[102]);
                
                _uinToUid[raw.Uin] = raw.Uid;
            }
        }
        
        if (_friends.TryGetValue(uin, out var friend)) return friend;
        
        throw new InvalidOperationException("干什么！");
    }
    
    private async Task<List<BotMemberContext>> GetMembers(uint groupUin, bool refreshCache = false)
    {
        if (refreshCache || !_members.TryGetValue(groupUin, out var members))
        {
            var memberList = new List<BotMemberContext>();
            string? token = null;
            
            do
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
                
                memberList.AddRange(body.Members.Select(member => new BotMemberContext(
                    (BotClient)_client,
                    member.Uin.Uin,
                    member.Uin.Uid,
                    (GroupMemberPermission)member.Permission,
                    member.Level?.Level ?? 0,
                    member.MemberCard.MemberCard,
                    member.MemberName,
                    DateTimeOffset.FromUnixTimeSeconds(member.JoinTimestamp).DateTime,
                    DateTimeOffset.FromUnixTimeSeconds(member.LastMsgTimestamp).DateTime)));

                foreach (var context in memberList) _uinToUid[context.Uin] = context.Uid;
                
                token = body.Token;
            } while (token != null);
            
            _members[groupUin] = memberList;
        }

        if (_members.TryGetValue(groupUin, out members)) return members;
        
        throw new InvalidOperationException("干什么！");
    }

    private async Task<BotGroupContext> GetGroups(uint groupUin, bool refreshCache = false)
    {
        if (refreshCache || !_groups.TryGetValue(groupUin, out var group))
        {
            var packet = new OidbSvcTrpcTcp0xFE5_2
            {
                Config = new OidbSvcTrpcTcp0xFE5_2Config
                {
                    Config1 = new OidbSvcTrpcTcp0xFE5_2Config1(),
                    Config2 = new OidbSvcTrpcTcp0xFE5_2Config2(),
                    Config3 = new OidbSvcTrpcTcp0xFE5_2Config3()
                }
            };
            
            var response = await _client.PacketHandler.SendOidb(0xfe5, 2, packet.Serialize(), false);
            var payload = response.Deserialize<OidbSvcBase>();
            var body = payload.Body?.Deserialize<OidbSvcTrpcTcp0xFE5_2Response>();
            
            if (body == null) throw new InvalidOperationException("干什么！");
            foreach (var raw in body.Groups) _groups[raw.GroupUin] = new BotGroupContext((BotClient)_client, raw.GroupUin, raw.Info.GroupName, raw.Info.MemberCount, raw.Info.MemberMax);
        }
        
        if (_groups.TryGetValue(groupUin, out group)) return group;

        throw new InvalidOperationException("干什么！");
    }
}