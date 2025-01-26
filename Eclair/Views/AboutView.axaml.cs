using Avalonia.Controls;
using System;

namespace Eclair.Views;

public partial class AboutView : UserControl
{
    public AboutView()
    {
        DataContext = new ViewModels.ViewModel(); // idk why, but the text is displayed correctly only this way
        InitializeComponent();

        if (OperatingSystem.IsAndroid())
        {
            Header.IsVisible = true;
            Header.Title = resources.ui_about;
            Header.GotoBack += (s, e) => Content = MainView.prevcontent;
        }
        else
            MainGrid.RowDefinitions[0].Height = new(0);
    }

    private async void GotoGitHubRepo(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var launcher = TopLevel.GetTopLevel(this)?.Launcher;

        if (launcher is null)
        {
            Logger.Log("launcher is null.", Notice);
            return;
        }

        bool success = await launcher.LaunchUriAsync(new("https://github.com/NonExistPlayer/EclairPlayer"));

        if (!success)
            Logger.Error("LaunchUriAsync() failed.");
    }
}