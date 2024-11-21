using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Eclair.ViewModels;
using Eclair.Views;
using System;
using System.Diagnostics;
using System.IO;

#pragma warning disable CA2211

namespace Eclair;

public partial class App : Application
{
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

    internal readonly static Config Config = Config.Load();

    public override void Initialize()
    { 
        if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);
        if (!Directory.Exists(TempPath)) Directory.CreateDirectory(TempPath);
        try
        {
            if (!Directory.Exists(LogPath))
                Directory.CreateDirectory(LogPath);
        }
        catch (UnauthorizedAccessException)
        {
            if (OperatingSystem.IsAndroid())
                LogPath = SavePath + "cache/logs/";
            else throw;
        }

#if DEBUG
        if (Process.GetCurrentProcess().ProcessName == "eclairplayer") // otherwise the program is launched for preview designer
            Logger.Init(
                new StreamWriter(LogPath +
                $"{DateTime.Now.ToString().Replace(':', '-').Replace(' ', '-')}.log")
            );
#endif

        AppDomain.CurrentDomain.ProcessExit += delegate
        {
            OnExit(0);
        };
        
        AppDomain.CurrentDomain.UnhandledException += UnhandledException;

        Environment.CurrentDirectory = SavePath;

        SetResources((AvaloniaXamlLoader.Load(new Uri("avares://Eclair/Assets/DefaultTheme.axaml")) as ResourceDictionary)!);

        Config.LoadResources();

        AvaloniaXamlLoader.Load(this);
    }

    private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Logger.WriteLine(e.ExceptionObject.ToString() ?? "<failed to get exception>", Error);
        try
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        catch { }
        OnExit(1);
        Environment.Exit(1);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    internal static void OnExit(int exitcode)
    {
        try
        {
            ClearTemp();
            Logger.WriteLine("All temporary files have been deleted.");
        }
        catch (Exception ex)
        {
            Logger.WriteLine("ClearTemp() threw an exception:\n" + ex.ToString(), Error);
        }
        try
        {
            Logger.WriteLine($"Application exited with code: {exitcode}\n---END-OF-LOG---");
#if DEBUG
            Logger.FileStream?.Close();
            if (OperatingSystem.IsWindows())
                WinApi.FreeConsole();
#endif
        }
        catch { }
    }

    internal void SetResources(ResourceDictionary resources)
    {
        foreach (var res in resources)
        {
            Resources.Remove(res.Key);
            Resources.Add(res);
        }
    }

    internal static void ClearTemp()
    {
        string[] files = Directory.GetFiles(TempPath);

        foreach (string file in files)
            File.Delete(file);
    }
}