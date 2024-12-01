using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Eclair.ViewModels;
using Eclair.Views;
using System;
using System.IO;

namespace Eclair;

public partial class App : Application
{
    public const string Version = "0.1.0.0";
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

    internal static IPlatformManager PManager = IPlatformManager.Null;

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

        //Logger.Init(
        //    new StreamWriter(LogPath +
        //    $"{DateTime.Now.ToString()
        //        .Replace(':', '-')
        //        .Replace(' ', '-')
        //        .Replace('/', '-')}.log")
        //);

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
                DataContext = new ViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new ViewModel()
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

    private static void ClearTemp()
    {
        string[] files = Directory.GetFiles(TempPath);

        foreach (string file in files)
            File.Delete(file);
    }

    internal static void ChangeView(UserControl view, UserControl sender)
    {
        if (!OperatingSystem.IsAndroid())
        {
            MainWindow win = new(view);
            win.Show();
        }
        else
            sender.Content = view;
    }
}