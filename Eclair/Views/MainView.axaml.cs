using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Eclair.Core;
using Eclair.ViewModels;
using Avalonia.Layout;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using static Eclair.ViewModels.MainViewModel;

namespace Eclair.Views;

public partial class MainView : UserControl
{
    MainViewModel ViewModel => (MainViewModel)DataContext!;
    readonly AudioPlayer player;

    public MainView()
    {
        InitializeComponent();
        player = new(true);

        player.AudioPlay +=
            (_, _) =>
                ViewModel.PlayIcon = CreateIcon(PauseIconValue);

        player.AudioPaused +=
            (_, _) =>
                ViewModel.PlayIcon = CreateIcon(PlayIconValue);

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

    #region Core Events
    static void PluginLoaded(object? sender, PluginLoadEventArgs e) =>
        Logger.Instance.Log($"Successfully loaded bass lib: {e.Library}, hnd: {e.Handle}.");
    static void PluginLoadFailed(object? sender, PluginLoadExceptionEventArgs e) =>
        Logger.Instance.Error($"Failed to load bass lib: {e.Library}.");
    void AudioLoaded(object? sender, IAudio audio)
    {
        Logger.Instance.Log($"Audio loaded: '{audio.LocalPath ?? audio.FileName}'.");
        
        if (audio is not Audio a)
            throw new UnreachableException();

        ViewModel.Title = a.Title;
        ViewModel.Performers = !string.IsNullOrEmpty(a.Performers) ? a.Performers : "-";
        ViewModel.Album = !string.IsNullOrEmpty(a.Album) ? a.Album : "-";
        ViewModel.ImageSource = a.Image?.Source;
    }
    #endregion

    #region Event Handlers
    void Play_Click(object? sender, RoutedEventArgs e)
    {
        if (player.IsPlaying) player.Pause();
        else                  player.Play();
    }

    void Next_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.Playlist.Player ??= player;
        ViewModel.Playlist.PlayNext();
    }

    void Prev_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.Playlist.Player ??= player;
        ViewModel.Playlist.PlayPrevious();
    }

    void List_Play_Click(object? sender, RoutedEventArgs re)
    {
        var button = (Button)sender!;
        var audio = (IAudio)button.Tag!;
    
        ViewModel.Playlist.Index = ViewModel.Playlist.IndexOf(audio);
        
        player.LoadAudio(audio);
        
        Logger.Instance.Log("Attaching events...");
        button.Click         -= List_Play_Click;
        button.Click         += Play_Click;
        player.AudioPlay     += Play;
        player.AudioPaused   += Paused;
        player.AudioUnloaded += Unloaded;
        
        player.Play();
        
        return;

        void Paused(object? s, EventArgs e) =>
            button.Content = CreateIcon(CirclePlayIconValue, 18);

        void Play(object? s, EventArgs e) =>
            button.Content = CreateIcon(CirclePauseIconValue, 18);

        void Unloaded(object? s, EventArgs e)
        {
            Logger.Instance.Log("Unattaching events...");
            button.Content = CreateIcon(CirclePlayIconValue, 18);
            button.Click         += List_Play_Click;
            button.Click         -= Play_Click;
            player.AudioPlay     -= Play;
            player.AudioPaused   -= Paused;
            player.AudioUnloaded -= Unloaded;
        }
    }
    
    void TracksContext_Remove(object? sender, RoutedEventArgs e)
    {
        var list = ViewModel.SelectedItems.ToImmutableList();
        foreach (var item in list)
            ViewModel.Playlist.Remove(item);
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
                AllowMultiple = true
            }
        );

        if (!files.Any()) return;
        
        foreach (var file in files)
            ViewModel.Playlist.Add(new Audio(file, await file.OpenReadAsync()));
    }
    #endregion
}