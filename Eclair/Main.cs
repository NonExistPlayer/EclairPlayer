﻿global using static Eclair.Main;
using NonExistPlayer.Logging;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Eclair;

public static class Main
{
    static Main()
    {
        Directory.CreateDirectory(SavePath);
        Config = ConfigJson.Load();
    }

    public const string Version = "0.3.0";
    #region Pathes
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
    #endregion

    #region Fields / Properties

    private static string? _osstr;
    public static string OSString
    {
        get
        {
            if (_osstr is null)
            {
                string arch = RuntimeInformation.ProcessArchitecture.ToString().ToLower();
                if (OperatingSystem.IsWindows())
                    _osstr = $"win-{arch}";
                else if (OperatingSystem.IsLinux())
                    _osstr = $"linux-{arch}";
                else if (OperatingSystem.IsAndroid())
                    _osstr = $"android-{arch}";
                else
                    _osstr = "none";
            }
            return _osstr;
        }
    }

    public readonly static string[] SupportedFormats = ["*.mp3", "*.mp2", "*.ogg", "*.aiff",
        "*.flac", "*.midi", "*.wv", "*.wvc", "*.opus", "*.dsd", "*.alac", "*.webm", "*.aac"];

    public static Logger<EclairLogLevel> Logger { get; } = new(Default)
    {
        OutputFormat = "[%time] (%class.%method/%TYPE): %mes",
        WarningLogLevel = new(3),
        ErrorLogLevel = new(4),
        WriteToConsole = true,
        WriteToDebug = true
    };

    internal readonly static ConfigJson Config;

    internal static IPlatformManager PManager = IPlatformManager.Null;
    #endregion

    #region Dates
    public readonly static DateTime NewYear_Start = new(DateTime.Now.Year, 12, 29);
    public readonly static DateTime NewYear_End   = new(DateTime.Now.Year, 1, 1);

    public readonly static DateTime EclairBirthday_Start = new(DateTime.Now.Year, 11, 12);
    public readonly static DateTime EclairBirthday_End   = new(DateTime.Now.Year, 11, 13);
    #endregion

    #region Methods
    internal static void ScanDirectoryForMusic(string targetdir, Action<string> finded) =>
        ScanDirectoryForMusic(targetdir, finded, 0);
    private static bool ScanDirectoryForMusic(string targetdir, Action<string> finded, int depth)
    {
        DirectoryInfo dirinfo = new(targetdir);

        if (!dirinfo.Exists) return false;
        if (depth > 10 ||
            dirinfo.Attributes.HasFlag(FileAttributes.ReparsePoint)) return true;

        string[] files;
        try
        {
            files = Directory.GetFiles(targetdir);
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return false;
        }

        foreach (string file in files)
        {
            if (HasSupportedFormat(file))
                finded(file);
        }

        // It is assumed that if the previous request to get files
        // in the directory was successful.
        // (otherwise the method would return an empty array)
        var dirs = dirinfo.GetDirectories();

        foreach (DirectoryInfo dir in dirs)
            if (!dir.Name.StartsWith('.'))
                if (ScanDirectoryForMusic(dir.FullName, finded, depth + 1))
                    break;
        
        return false;
    }

    internal static bool HasSupportedFormat(string fname)
    {
        foreach (string format in SupportedFormats)
            if (fname.EndsWith(format[1..])) return true;
        return false;
    }

    internal static void LoggerInit()
    {
        if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);
        if (!Directory.Exists(TempPath)) Directory.CreateDirectory(TempPath);
        try
        {
            if (!Directory.Exists(LogPath))
                Directory.CreateDirectory(LogPath);
        }
        catch (UnauthorizedAccessException) when (OperatingSystem.IsAndroid())
        {
            LogPath = SavePath + "cache/logs/";
            if (!Directory.Exists(LogPath))
                Directory.CreateDirectory(LogPath);
        }

        Environment.CurrentDirectory = SavePath;

        var files = Directory.GetFiles(LogPath);
        if (files.Length > 15)
        {
            foreach (string file in files[15..])
                File.Delete(file);
        }

        DateTime now = DateTime.Now;
        string path = LogPath + $"{now.Month}-{now.Day}_{now.Hour}-{now.Minute}-{now.Second}.log";
        Logger.FileStream = new StreamWriter(path);

        Logger.Log($"Log Path: {path}");
    }
    #endregion
}