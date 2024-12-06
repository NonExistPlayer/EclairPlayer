using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace Eclair.Views;

public partial class MainWindow : Window
{
    internal static List<MainWindow> OtherWindows = [];
    
    public MainWindow()
    {
        InitializeComponent();
        InitializeBackground();
        MinWidth = 790;
        MinHeight = 670;
        View.Content = new MainView();
    }

    public MainWindow(UserControl view)
    {
        InitializeComponent();
        InitializeBackground();
        View.Content = view;
        Width = 600;
        Height = 450;
        CanResize = false;

        OtherWindows.Add(this);
    }

    private void InitializeBackground()
    {
        Background = new SolidColorBrush(Config.BackgroundColor);
        if (OperatingSystem.IsWindows() && !Config.DisableCustomBorder)
        {
            ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome;
            ExtendClientAreaToDecorationsHint = true;
        }
        else
        {
            BorderPanel.IsVisible = false;
            MainGrid.RowDefinitions.RemoveAt(1);
            MainGrid.RowDefinitions[0].Height = GridLength.Star;
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        if (Width != 600 /*other windows are always this width*/)
        {
            Logger.Log("Closing other windows...");
            for (int i = 0; i < OtherWindows.Count; i++)
            {
                var win = OtherWindows[i];
                Logger.Log($"Closing window {win} with index {i}...");
                win.Close();
            }
        }
        else
            OtherWindows.Remove(this);
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender as Button is not Button button) return;
        if ((string?)button.Content == "-")
            WindowState = WindowState.Minimized;
        else
            Close();
    }
}