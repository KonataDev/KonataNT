using KonataNT.Message;

namespace KonataNT.Common.Context;

[Serializable]
public class BotFriendContext : IBotContactable
{
    #region Properties

    internal BotFriendContext(BotClient client, uint uin, string uid, string nickname, string remarks, string personalSign)
    {
        Client = client;
        Uin = uin;
        Uid = uid;
        Nickname = nickname;
        Remarks = remarks;
        PersonalSign = personalSign;
    }

    public BotClient Client { get; }

    public uint Uin { get; set; }
    
    internal string Uid { get; set; }
    
    public string Nickname { get; set; }
    
    public string Remarks { get; set; }
    
    public string PersonalSign { get; set; }

    public string Avatar => $"https://q1.qlogo.cn/g?b=qq&nk={Uin}&s=640";

    #endregion
    
    public void SendLike(int count)
    {
        throw new NotImplementedException();
    }
    
    public async Task<MessageStruct> GetHistoryMessage(uint sequence, uint count)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SendFile(Stream stream)
    {
        throw new NotImplementedException();
    }
    
    public Task<MessageResult> SendMessage()
    {
        throw new NotImplementedException();
    }
}