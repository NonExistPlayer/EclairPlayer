using System;
using System.Collections.Generic;
using System.Diagnostics;
using Eclair.Core;
using Eclair.ViewModels;
using Avalonia.Layout;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;

namespace Eclair.Views;

public partial class MainView : UserControl
{
    readonly List<Audio> playlist = [];
    MainViewModel ViewModel => (MainViewModel)DataContext!;
    readonly AudioPlayer player;

    public MainView()
    {
        InitializeComponent();
        player = new(true);
        Logger.Instance.Log("Bass initialized.");

        player.AudioPlay +=
            (_, _) =>
                PlayButton.Content = ViewModel.PauseIcon;

        player.AudioPaused +=
            (_, _) =>
                PlayButton.Content = ViewModel.PlayIcon;

        player.AudioLoaded += AudioLoaded;
        player.PluginLoaded += PluginLoaded;
        player.PluginLoadFailed += PluginLoadFailed;

        player.PluginLoad("aac" );
        player.PluginLoad("alac");
        player.PluginLoad("dsd" );
        player.PluginLoad("flac");
        player.PluginLoad("midi");
        player.PluginLoad("opus");
        player.PluginLoad("webm");
        player.PluginLoad("wv"  );
    }

    void AddAudioToPanel(Audio audio)
    {
        if (playlist.Contains(audio))
        {
            Logger.Instance.Log("Audio already in playlist, skipping.");
            return;
        }
        
        var border = new Border
        {
            CornerRadius = new(5),
            Margin = new(5),
            Background = Brushes.Gray,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        var panel = new DockPanel();

        Image? img = null;

        if (audio.Image != null)
        {
            img = new()
            {
                Source = audio.Image.Source,
                MaxWidth = 47,
                Margin = new(5, 0),
            };
        }

        var title = new TextBlock
        {
            Text = audio.FullTitle,
            FontWeight = ReferenceEquals(player.CurrentAudio, audio) ? FontWeight.Bold : FontWeight.Normal,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var playButton = new Button
        {
            Content = ViewModel.PlayIcon
        };

        playButton.Click += (_, _) =>
        {
            if (ReferenceEquals(player.CurrentAudio, audio))
                player.Stop();
            else
                player.LoadAudio(audio);
            player.Play();
        };
        
        if (img != null)
            panel.Children.Add(img);
        panel.Children.AddRange([playButton, title]);
        border.Child = panel;
        TracksPanel.Children.Add(border);
        playlist.Add(audio);
    }

    #region Core Events
    static void PluginLoaded(object? sender, PluginLoadEventArgs e) =>
        Logger.Instance.Log($"Successfully loaded bass lib: {e.Library}, hnd: {e.Handle}.");
    static void PluginLoadFailed(object? sender, PluginLoadExceptionEventArgs e) =>
        Logger.Instance.Error($"Failed to load bass lib: {e.Library}.");
    void AudioLoaded(object? sender, IAudio audio)
    {
        Logger.Instance.Log($"Audio loaded: '{audio.LocalPath ?? audio.FileName}'.");
        ViewModel.Title = audio.FullTitle;

        if (audio is not Audio a)
            throw new UnreachableException();
                
        ViewModel.ImageSource = a.Image?.Source;
    }
    #endregion

    #region Event Handlers
    void Play_Click(object? sender, RoutedEventArgs e)
    {
        if (player.IsPlaying) player.Pause();
        else                  player.Play();
    }

    // ReSharper disable once AsyncVoidMethod
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
            var audio = new Audio(file, await file.OpenReadAsync());
            player.LoadAudio(audio);
            AddAudioToPanel(audio);
        }
    }
    #endregion
}