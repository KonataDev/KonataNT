using KonataNT.Message;

namespace KonataNT.Common.Context;

public interface IBotContactable
{
    protected BotClient Client { get; }

    public Task<MessageResult> SendMessage();
}