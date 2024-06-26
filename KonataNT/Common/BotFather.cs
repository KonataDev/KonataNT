namespace KonataNT.Common;

public class BotFather
{
    /// <summary>
    /// Create the <see cref="BotClient"/> with default <see cref="BotKeystore"/>
    /// </summary>
    public static BotClient CreateBot(BotConfig config, uint uin, string password)
    {
        return new BotClient(new BotKeystore(uin, password), config);
    }

    /// <summary>
    /// Create the <see cref="BotClient"/> with saved <see cref="BotKeystore"/> to preform SessionLogin and EasyLogin
    /// </summary>
    public static BotClient CreateBot(BotConfig config, BotKeystore keystore) => new(keystore, config);
}