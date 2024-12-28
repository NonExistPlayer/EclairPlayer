// This file is responsible for the fullscreen player in MainView.
using LibVLCSharp.Shared;
using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Svg.Skia;
using Avalonia.Threading;

namespace Eclair.Views;

partial class MainView
{
    #region Events
    private async void PlayButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (player.Media == null) await GetMusicFile();
        if (player.Media != null)
            PlayOrPause();
    }
    private void Player_PositionChanged(object? sender, MediaPlayerPositionChangedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(delegate
        {
            calledByPlayer = true;
            MusSlider.Value = e.Position * 100;
        });
    }
    private void StopButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Stop();
    private void SliderValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        MusPositionLabel.Content = TimeSpan.FromMilliseconds((double)(player.Media!.Duration * player.Position)).ToString(@"mm\:ss");
        if (calledByPlayer)
        {
            calledByPlayer = false;
            return;
        }
        player.Position = (float)(e.NewValue / 100);
    }
    private void LB_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        loop = !loop;
        if (Application.Current == null) return;
        Dispatcher.UIThread.Invoke(delegate
        {
            LB_Image.Source = (SvgImage?)Application.Current.Resources[(loop ? "selected" : "") + "looparrow"];
        });
    }
    #endregion

    private async Task GetMusicFile()
    {
        bool playing = player.IsPlaying;
        if (playing)
            PlayOrPause();

        var toplevel = TopLevel.GetTopLevel(this); if (toplevel == null) return;
        var files = await toplevel.StorageProvider.OpenFilePickerAsync(
        new FilePickerOpenOptions
        {
            Title = resources.ui_ofd_title,
            AllowMultiple = false,
            FileTypeFilter = [
                new(resources.ui_ofd_audiofiles)
                    {
                        Patterns = SupportedFormats,
                        MimeTypes = ["audio/*"]
                    }
            ]
        });

        if (files == null || files.Count != 1)
        {
            if (playing)
                PlayOrPause();
            return;
        }

        LoadMusicFile(files[0].Name, await files[0].OpenReadAsync());
    }

    private void PlayOrPause()
    {
        PlayButtonSetImage(player.IsPlaying ? "play" : "pause");

        if (player.IsPlaying)
        {
            player.Pause();
            PManager.ShowPlayerNotification(TitleLabel.Content?.ToString()!, false);
        }
        else
        {
            if (player.Position == 0) player.Stop();

            player.Play();

            PManager.ShowPlayerNotification(TitleLabel.Content?.ToString()!, true);
        }
    }
    private void Stop()
    {
        PlayButtonSetImage("play");
        Dispatcher.UIThread.Invoke(delegate
        {
            MusDurationLabel.Content = "00:00";
            MusSlider.Value = 0;
            if (rttransform != null) rttransform.Angle = 0;
        });
        PManager.HidePlayerNotification();

        player.Stop();
    }
}