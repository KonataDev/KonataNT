using KonataNT.Core;

namespace KonataNT.Common;

public class BotConfig
{
    public bool AutoReconnect { get; set; } = true;
    
    public Protocols Protocol { get; set; }
    
    public ILogger? Logger { get; set; }
}

public enum Protocols
{
    Windows = 0,
    MacOs = 1,
    Linux = 2
}