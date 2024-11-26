using Avalonia.Controls;
using System;

namespace Eclair.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();

        if (OperatingSystem.IsAndroid())
            Sections.IsVisible = false;
    }

    private void GotoBack(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Content = MainView.prevcontent;
}