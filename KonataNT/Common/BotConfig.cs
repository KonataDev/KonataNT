using KonataNT.Core;

namespace KonataNT.Common;

public class BotConfig
{
    public bool AutoReconnect { get; set; } = true;
    
    public ILogger? Logger { get; set; }
}