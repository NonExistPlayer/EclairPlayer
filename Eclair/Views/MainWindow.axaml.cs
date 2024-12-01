using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace Eclair.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Background = new SolidColorBrush(new Color(125, 0, 0, 0));
        if (OperatingSystem.IsWindows())
        {
            ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome;
            ExtendClientAreaToDecorationsHint = true;
        }
        else BorderPanel.IsVisible = false;
    }

    public MainWindow(UserControl view)
    {
        InitializeComponent();

        Content = view;
        MaxWidth = 600;
        MaxHeight = 450;
        Width = 600;
        Height = 450;
        CanResize = false;
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