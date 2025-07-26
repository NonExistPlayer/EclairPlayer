using NonExistPlayer.Logging;

namespace Eclair;

public static class Logger
{
    public static Logger<LogLevel> Instance { get; } = new(LogLevel.Info)
    {
        OutputFormat = "[{6}] [{5}/{3}.{1}()] {0}",
        WriteToConsole = true,
        WarningLogLevel = LogLevel.Warning,
        ErrorLogLevel = LogLevel.Error
    };

    public static void Log(object value, ILogLevel? loglevel = null) => Instance.Log(value, loglevel);
    public static void Warn(object value) => Instance.Warn(value);
    public static void Error(object value) => Instance.Error(value);
}