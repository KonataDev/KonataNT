using KonataNT.Common;
using KonataNT.Common.Context;
using KonataNT.Core;
using KonataNT.Core.Packet.Oidb;
using KonataNT.Core.Packet.Service.Oidb;
using KonataNT.Utility;

namespace KonataNT;

public class BotClient : BaseClient
{
    internal BotClient(BotKeystore keystore, BotConfig config) : base(keystore, config) { }

    public async Task<string> FetchClientKey()
    {
        var response = await PacketHandler.SendOidb(0x102a, 1, Array.Empty<byte>(), false);
        var payload = response.Deserialize<OidbSvcBase>();
        var body = payload.Body?.Deserialize<OidbSvcTrpcTcp0x102A_1Response>();
        
        if (body != null) return body.ClientKey;
        
        throw new Exception("干什么！");
    }

    public async Task<List<BotGroupContext>> GetGroups(bool refreshCache = false)
    {
        throw new NotImplementedException();
    }
    
    public async Task<List<BotMemberContext>> GetMembers(uint groupUin, bool refreshCache = false)
    {
        throw new NotImplementedException();
    }

    public async Task<BotFriendContext> GetFriends(bool refreshCache = false)
    {
        throw new NotImplementedException();
    }
}