global using static Eclair.Main;
using Newtonsoft.Json;
using NonExistPlayer.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Eclair;

public static class Main
{
    public const string Version = "0.1.1";
    public static string SavePath { get; } = OperatingSystem.IsWindows() ?
        $"{Environment.GetEnvironmentVariable("LOCALAPPDATA")}\\EclairPlayer\\" : (OperatingSystem.IsAndroid() ?
        "/data/data/net.nonexistplayer.eclair/files/" :
        $"{Environment.GetEnvironmentVariable("HOME")}/.eclairplayer/" // linux
        );
    public static string LogPath { get; internal set; } = OperatingSystem.IsAndroid() ?
        "/storage/emulated/0/Android/data/net.nonexistplayer.eclair/cache/logs/" :
        SavePath + $"logs{Path.DirectorySeparatorChar}";

    public static string TempPath { get; } = OperatingSystem.IsWindows() ?
        $"{Environment.GetEnvironmentVariable("TEMP")}\\EclairPlayer\\" : (OperatingSystem.IsAndroid() ?
        "/data/data/net.nonexistplayer.eclair/cache/" :
        "/tmp/eclairplayer/"
        );

    public readonly static string[] SupportedFormats = ["*.mp3", "*.aac", "*.asf", "*.wma", "*.ogg", "*.flac", "*.flv", "*.midi"];

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

    internal static void ScanDirectoryForMusic(string targetdir, Action<string> finded)
    {
        if (!Directory.Exists(targetdir)) return;

        string[] files;
        try
        {
            files = Directory.GetFiles(targetdir);
        }
        catch (UnauthorizedAccessException)
        {
            return;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return;
        }

        foreach (string file in files)
        {
            if (HasSupportedFormat(file))
                finded(file);
        }

        // It is assumed that if the previous request to get files
        // in the directory was successful.
        // (otherwise the method would return an empty array)
        string[] dirs = Directory.GetDirectories(targetdir);

        foreach (string dir in dirs)
            if (!Path.GetFileName(dir)!.StartsWith('.'))
                ScanDirectoryForMusic(Path.GetFullPath(dir), finded);
    }

    internal static bool HasSupportedFormat(string fname)
    {
        foreach (string format in SupportedFormats)
            if (fname.EndsWith(format[1..])) return true;
        return false;
    }
}