using Avalonia.Controls;
using System;

namespace Eclair.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(bool settings)
    {
        InitializeComponent();

        if (settings)
        {
            Content = new SettingsView();
            Width = 600;
            Height = 450;
            MaxWidth = 600;
            MaxHeight = 450;
            CanResize = false;
        }
    }

    internal void ShowSettings()
    {
        Logger.WriteLine("ShowSettings()");
        if (!OperatingSystem.IsAndroid())
        {
            var win = new MainWindow(true);
            win.Show();
        }
        else
            Content = new SettingsView();
    }
}