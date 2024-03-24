namespace KonataNT.Common;

public class BotFather
{
    /// <summary>
    /// Create the <see cref="BotClient"/> with default <see cref="BotKeystore"/>
    /// </summary>
    public static BotClient CreateBot(BotConfig config, uint uin, string password)
    {
        return new BotClient();
    }

    /// <summary>
    /// Create the <see cref="BotClient"/> with saved <see cref="BotKeystore"/> to preform SessionLogin and EasyLogin
    /// </summary>
    public static BotClient CreateBot(BotConfig config, BotKeystore keystore)
    {
        return new BotClient();
    }
}