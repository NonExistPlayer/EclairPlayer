global using static Eclair.Main;
using NonExistPlayer.Logging;
using System;
using System.IO;

namespace Eclair;

internal static class Main
{
    public const string Version = "0.1.1";
    public readonly static string SavePath = OperatingSystem.IsWindows() ?
        $"{Environment.GetEnvironmentVariable("LOCALAPPDATA")}\\EclairPlayer\\" : (OperatingSystem.IsAndroid() ?
        "/data/data/net.nonexistplayer.eclair/files/" :
        $"{Environment.GetEnvironmentVariable("HOME")}/.eclairplayer/" // linux
        );
    public static string LogPath = OperatingSystem.IsAndroid() ?
        "/storage/emulated/0/Android/data/net.nonexistplayer.eclair/cache/logs/" :
        SavePath + $"logs{Path.DirectorySeparatorChar}";

    public readonly static string TempPath = OperatingSystem.IsWindows() ?
        $"{Environment.GetEnvironmentVariable("TEMP")}\\EclairPlayer\\" : (OperatingSystem.IsAndroid() ?
        "/data/data/net.nonexistplayer.eclair/cache/" :
        "/tmp/eclairplayer/"
        );

    public static Logger<EclairLogLevel> Logger { get; } = new(Default)
    {
        OutputFormat = "[%time] (%class.%method/%TYPE): %mes",
        WarningLogLevel = new(3),
        ErrorLogLevel = new(4),
        WriteToConsole = true,
        WriteToDebug = true
    };

    internal readonly static ConfigJson Config = ConfigJson.Load();

    internal static IPlatformManager PManager = IPlatformManager.Null;
}