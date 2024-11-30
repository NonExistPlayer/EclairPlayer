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
            BackButton.Click += delegate
            {
                Content = MainView.prevcontent;
            };
        }
        else BackButton.IsVisible = false;
    }

    private async void GotoGitHubRepo(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var launcher = TopLevel.GetTopLevel(this)?.Launcher;

        if (launcher is null)
        {
            Logger.WriteLine("launcher is null.", Notice);
            return;
        }

        bool success = await launcher.LaunchUriAsync(new("https://github.com/NonExistPlayer/EclairPlayer"));

        if (!success)
            Logger.WriteLine("LaunchUriAsync() failed.", Error);
    }
}