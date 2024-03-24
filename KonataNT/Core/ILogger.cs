using KonataNT.Events;
using KonataNT.Events.EventArgs;

namespace KonataNT.Core;

public interface ILogger
{
    void Log(string tag, LogLevel level, string message);
    
    void LogDebug(string tag, string message) => Log(tag, LogLevel.Debug, message);
    
    void LogInformation(string tag, string message) => Log(tag, LogLevel.Information, message);
    
    void LogWarning(string tag, string message) => Log(tag, LogLevel.Warning, message);
    
    void LogError(string tag, string message) => Log(tag, LogLevel.Error, message);
    
    void LogFatal(string tag, string message) => Log(tag, LogLevel.Fatal, message);
}


public class DefaultLogger(EventEmitter emitter) : ILogger
{
    public void Log(string tag, LogLevel level, string message) => emitter.PostEvent(new BotLogEvent(tag, level, message));
}


public enum LogLevel
{
    Debug,
    Information,
    Warning,
    Error,
    Fatal
}