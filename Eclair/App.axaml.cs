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
    public override void Initialize()
    {
        AppDomain.CurrentDomain.ProcessExit += delegate
        {
            OnExit(Environment.ExitCode);
        };

        AppDomain.CurrentDomain.UnhandledException += UnhandledException;

        if (!OperatingSystem.IsAndroid())
            LoggerInit();

        SetResources((AvaloniaXamlLoader.Load(new Uri("avares://Eclair/Assets/DefaultTheme.axaml")) as ResourceDictionary)!);

        Config.LoadResources();

        AvaloniaXamlLoader.Load(this);
    }

    private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Logger.Error(e.ExceptionObject.ToString() ?? "<failed to get exception>");
        try
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        catch { }
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
            Logger.Log("All temporary files have been deleted.");
        }
        catch (Exception ex)
        {
            Logger.Error("ClearTemp() threw an exception:\n" + ex.ToString());
        }
        try
        {
            Logger.Log($"Application exited with code: {exitcode}\n---END-OF-LOG---");
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