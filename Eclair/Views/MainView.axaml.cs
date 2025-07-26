using System;
using Eclair.Core;
using Eclair.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Eclair.Views;

public partial class MainView : UserControl
{
    MainViewModel ViewModel => (MainViewModel)DataContext!;
    public AudioPlayer Player { get; }

    public MainView()
    {
        InitializeComponent();
        Player = new(true);
        Logger.Instance.Log("Player initialized.");

        Player.AudioPlay +=
            (sender, e) =>
                PlayButton.Content = ViewModel.PauseIcon;

        Player.AudioPaused +=
            (sender, e) =>
                PlayButton.Content = ViewModel.PlayIcon;

        Player.PluginLoaded +=
            (sender, e) =>
                Logger.Instance.Log($"Successfully loaded bass lib: {e.Library}, hnd: {e.Handle}.");

        Player.PluginLoadFailed +=
            (sender, e) =>
                Logger.Instance.Error($"Failed to load bass lib: {e.Library}.");

        Player.PluginLoad("aac" );
        Player.PluginLoad("alac");
        Player.PluginLoad("dsd" );
        Player.PluginLoad("flac");
        Player.PluginLoad("midi");
        Player.PluginLoad("opus");
        Player.PluginLoad("webm");
        Player.PluginLoad("wv"  );
    }

    void Play_Click(object? sender, RoutedEventArgs e)
    {
        if (Player.IsPlaying) Player.Pause();
        else                  Player.Play();
    }

    async void Open_Click(object? sender, RoutedEventArgs e)
    {
        var toplevel = TopLevel.GetTopLevel(this);

        var files = await toplevel!.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                FileTypeFilter = [
                    new("MPEG-1 Audio Layer III")
                    {
                        Patterns = ["*.mp3"],
                        MimeTypes = ["audio/mpeg"]
                    }
                ],
                AllowMultiple = false
            }
        );

        if (files.Count == 1)
        {
            var file = files[0];
            Player.LoadAudio(new Audio(file, await file.OpenReadAsync()));
        }
    }
}