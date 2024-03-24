using KonataNT.Common;
using KonataNT.Core;

namespace KonataNT;

public class BotClient : BaseClient
{
    internal BotClient(BotKeystore keystore, BotConfig config) : base(keystore, config) { }
}