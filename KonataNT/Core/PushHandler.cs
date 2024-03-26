using KonataNT.Events;
    
namespace KonataNT.Core;

internal class PushHandler
{
    private readonly BaseClient _client;
    
    private Dictionary<string, Action<byte[]>> _handlerFunction;

    public PushHandler(BaseClient client)
    {
        _client = client;
        _handlerFunction = new Dictionary<string, Action<byte[]>>
        {
            { "trpc.msg.olpush.OlPushService.MsgPush", ParseMsfPush },
            { "trpc.qq_new_tech.status_svc.StatusService.KickNT", ParseMsfKick }
        };
    }
    
    private void ParseMsfPush(byte[] packet)
    {
        throw new NotImplementedException();
    }

    private void ParseMsfKick(byte[] packet)
    {
        throw new NotImplementedException();
    }
}