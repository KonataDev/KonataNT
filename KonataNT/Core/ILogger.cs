using KonataNT.Events;

namespace KonataNT.Core;

public interface ILogger
{
    void Log(LogLevel level, string message);
    
    void LogDebug(string message) => Log(LogLevel.Debug, message);
    
    void LogInformation(string message) => Log(LogLevel.Information, message);
    
    void LogWarning(string message) => Log(LogLevel.Warning, message);
    
    void LogError(string message) => Log(LogLevel.Error, message);
    
    void LogFatal(string message) => Log(LogLevel.Fatal, message);
}


public class DefaultLogger(EventEmitter emitter) : ILogger
{
    public void Log(LogLevel level, string message)
    {
        throw new NotImplementedException();
    }
}


public enum LogLevel
{
    Debug,
    Information,
    Warning,
    Error,
    Fatal
}