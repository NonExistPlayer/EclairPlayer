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
}