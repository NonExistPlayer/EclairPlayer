// This file is responsible for the fullscreen player in MainView.
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
        if (shnd == 0)
        {
            if (MusicPanel.Children.Count == 1 &&
                MusicPanel.Children[0] is TextBlock)
                await GetMusicFile();

            PlayTrack(0);
        }
        if (shnd != 0)
            PlayOrPause();
    }
    private void PreviousClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => PlayPrevious();
    private void SkipForwardClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => PlayNext();
    private void StopButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Stop();
    
    bool sliderPressed;
    private void SliderValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        MusPositionLabel.Content = TimeSpan.FromSeconds(!sliderPressed ? CurrentPos : MusSlider.Value).ToString(@"mm\:ss");
        if (calledByPlayer)
        {
            calledByPlayer = false;
            return;
        }
        sliderPressed = true;
    }
    private void SliderPointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e) 
    {
        CurrentPos = MusSlider.Value;
        sliderPressed = false;
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
        bool playing = isplaying;
        if (playing)
            PlayOrPause();

        var toplevel = TopLevel.GetTopLevel(this); if (toplevel == null) return;
        var files = await toplevel.StorageProvider.OpenFilePickerAsync(
        new FilePickerOpenOptions
        {
            Title = resources.ui_ofd_title,
            AllowMultiple = true,
            FileTypeFilter = [
                new(resources.ui_ofd_audiofiles)
                    {
                        Patterns = SupportedFormats,
                        MimeTypes = ["audio/*"]
                    }
            ]
        });

        if (files.Count < 1)
        {
            if (playing)
                PlayOrPause();
            return;
        }

        foreach (var file in files)
        {
            var stream = await file.OpenReadAsync();
            AddMusicItem(file.Name, stream);

            if (files[0] == file)
                LoadMusicFile(file.Name, stream);
        }
    }

    private void PlayOrPause()
    {
        if (isplaying)
        {
            Pause();
            PManager.ShowPlayerNotification(TitleLabel.Content?.ToString()!, false);
        }
        else
        {
            Play();

            PManager.ShowPlayerNotification(TitleLabel.Content?.ToString()!, true);

            if (!timer.IsEnabled)
                timer.Start();
        }

        PlayButtonSetImage(isplaying ? "pause" : "play");
    }
}