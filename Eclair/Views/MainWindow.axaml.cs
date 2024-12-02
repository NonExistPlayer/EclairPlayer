using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace Eclair.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Background = new SolidColorBrush(App.Config.BGColor);
        MinWidth = 790;
        MinHeight = 670;
        if (OperatingSystem.IsWindows() && !App.Config.DisableCustomBorder)
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

    public MainWindow(UserControl view)
    {
        InitializeComponent();

        Content = view;
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