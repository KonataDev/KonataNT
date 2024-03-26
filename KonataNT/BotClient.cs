using KonataNT.Common;
using KonataNT.Common.Context;
using KonataNT.Core;

namespace KonataNT;

public class BotClient : BaseClient
{
    internal BotClient(BotKeystore keystore, BotConfig config) : base(keystore, config) { }

    public async Task<string> FetchClientKey()
    {
        var response = await PacketHandler.SendOidb(0x102a, 1, Array.Empty<byte>(), false);

        return string.Empty;
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