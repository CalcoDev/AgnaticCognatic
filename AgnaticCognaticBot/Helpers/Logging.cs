using Discord;
using NLog;

namespace AgnaticCognaticBot.Helpers;

public abstract class Logging
{
    public static Task Log(LogMessage logMessage, Logger logger)
    {
        string message = $"{logMessage.Source}: {logMessage.Message} {logMessage.Exception}";
        
        switch (logMessage.Severity)
        {
            case LogSeverity.Critical:
                logger.Fatal(message);
                break;
            case LogSeverity.Error:
                logger.Error(message);
                break;
            case LogSeverity.Warning:
                logger.Warn(message);
                break;
            case LogSeverity.Info:
                logger.Info(message);
                break;
            case LogSeverity.Verbose:
                break;
            case LogSeverity.Debug:
                logger.Debug(message);
                break;
            default:
                logger.Warn("Unknown log severity! " + message);
                break;
        }
        
        return Task.CompletedTask;
    }
}