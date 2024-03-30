using KonataNT.Core.Packet.Message;
using KonataNT.Message;
using KonataNT.Utility;

namespace KonataNT.Core.Handlers;

internal class PushHandler
{
    private const string Tag = nameof(PushHandler);
    
    private readonly BaseClient _client;
    
    private readonly Dictionary<string, Action<byte[]>> _handlerFunction;

    public PushHandler(BaseClient client)
    {
        _client = client;
        _handlerFunction = new Dictionary<string, Action<byte[]>>
        {
            { "trpc.msg.olpush.OlPushService.MsgPush", ParseMsfPush },
            { "trpc.qq_new_tech.status_svc.StatusService.KickNT", ParseMsfKick }
        };
    }
    
    public void HandlePush(string service, byte[] packet)
    {
        if (_handlerFunction.TryGetValue(service, out var value)) value(packet);
    }
    
    private void ParseMsfPush(byte[] packet)
    {
        var msgPush = packet.Deserialize<PushMsg>();

        switch (msgPush.Message.ContentHead.Type)
        {
            case 166: // private msg
            case 82: // group msg
            {
                var eventArg = MessagePacker.Parse(msgPush.Message);
                _client.EventEmitter.PostEvent(eventArg);
                break;
            }
            default:
            {
                _client.Logger.LogInformation(Tag, $"Unsupported message type: {msgPush.Message.ContentHead.Type}");
                break;
            }
        }
    }

    private void ParseMsfKick(byte[] packet)
    {
        throw new NotImplementedException();
    }
}