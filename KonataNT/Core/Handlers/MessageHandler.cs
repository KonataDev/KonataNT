using KonataNT.Core.Packet.Message;
using KonataNT.Events;
using KonataNT.Events.EventArgs;
using KonataNT.Message;
using KonataNT.Message.Chains;

namespace KonataNT.Core.Handlers;

internal class MessageHandler(BaseClient client)
{
    private const string Tag = nameof(MessageHandler);
    
    public EventBase Parse(PushMsgBody msg)
    {
        string name = msg.ResponseHead.Grp?.MemberCard ?? "";
        uint timestamp = msg.ContentHead.Timestamp;
        var @struct = new MessageStruct(msg.ResponseHead.FromUin, name, DateTime.Now)
        {
            Time = DateTime.UnixEpoch.AddSeconds(timestamp),
            Sequence = msg.ContentHead.Sequence,
            SourceType = msg.ResponseHead.Grp is not null ? MessageStruct.Source.Group : MessageStruct.Source.Friend
        };

        if (msg.Body.RichText?.Elems is { } elems)
        {
            foreach (var elem in elems)
            {
                switch (elem)
                {
                    case { Text: { } text }:
                    {
                        if (@struct.Chain.FirstOrDefault(x => x is TextChain) is { } existing)
                        {
                            ((TextChain)existing).Combine(TextChain.Create(text.Str ?? ""));
                        }
                        else
                        {
                            @struct.Chain.Add(TextChain.Create(text.Str ?? ""));
                        }
                        break;
                    }
                    case { NotOnlineImage: { } image }:
                    {
                        break;
                    }
                    case { CustomFace: { } image }:
                    {
                        break;
                    }
                    case { CommonElem: { } common }:
                    {
                        switch (common.BusinessType)
                        {
                            
                        }
                        
                        break;
                    }
                }
            }
        }
        
        client.Logger.LogInformation(Tag, $"Received message from {name} ({msg.ResponseHead.FromUin})");

        return @struct.SourceType switch
        {
            MessageStruct.Source.Group => new BotGroupMessageEvent(msg.ResponseHead.Grp?.GroupUin ?? 0, msg.ResponseHead.FromUin, @struct),
            MessageStruct.Source.Friend => new BotPrivateMessageEvent(msg.ResponseHead.FromUin, @struct),
            _ => throw new NotImplementedException()
        };
    }

    public MessageChain Build(BotClient client, MessageStruct @struct)
    {
        throw new NotImplementedException();
    }
}