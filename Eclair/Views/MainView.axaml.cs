﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Svg.Skia;
using Avalonia.Threading;
using LibVLCSharp.Shared;
using System;
using System.IO;
using System.Threading.Tasks;
using TagLib;
using File = System.IO.File;
using TagFile = TagLib.File;
using Avalonia.Media;
using System.Threading;
using Avalonia.Controls.ApplicationLifetimes;

namespace Eclair.Views;

public partial class MainView : UserControl
{
    internal static object? prevcontent;

    readonly LibVLC vlc;
    internal MediaPlayer player;
    readonly CancellationTokenSource ctsource = new();
    bool calledByPlayer;

    bool loop = false;

    RotateTransform? rttransform;

    public MainView()
    {
        InitializeComponent();
        vlc = new();
        vlc.Log += LibVlcOutput;

        player = new(vlc);

        player.EndReached += delegate
        {
            Dispatcher.UIThread.InvokeAsync(delegate
            {
                MusSlider.Value = 0;
                if (loop)
                {
                    PlayOrPause();
                    ctsource.Cancel();
                }
                else PlayButtonSetImage("play");
            });
            if (PManager != null && !loop)
                PManager.Stop!();
        };
        player.PositionChanged += Player_PositionChanged;

        player.Playing += delegate
        {
            Dispatcher.UIThread.Invoke(delegate
            {
                if (Config.UseCircleIconAnimation)
                    Task.Run(AnimateIcon, ctsource.Token);

                MusDurationLabel.Content = TimeSpan.FromMilliseconds(player.Media!.Duration).ToString(@"mm\:ss");
            });
        };

        if (Config.UseCircleIconAnimation)
            rttransform = MusicPicture.RenderTransform as RotateTransform;
        else
            MusicPicture.Clip = null;

        PManager.TogglePause += PlayOrPause;
        PManager.Stop += Stop;
    }

    private void LibVlcOutput(object? sender, LogEventArgs e) => Logger.Log(e.Message, new((ushort)e.Level));

    private async void PlayButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (player.Media == null) await GetMusicFile();
        if (player.Media != null)
            PlayOrPause();
    }

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
                        Patterns = ["*.mp3", "*.aac", "*.asf", "*.wma", "*.ogg", "*.flac", "*.flv", "*.midi"],
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

        MusDurationLabel.Content = "00:00";

        if (player.Media == null)
        {
            StopButton.IsEnabled = true;
            MusSlider.IsEnabled = true;
        }
        else
        {
            Stop();
            player.Media.Dispose();
        }
        
        Stream stream = await files[0].OpenReadAsync();

        var file = TagFile.Create(new ReadOnlyFileImplementation(files[0].Name, stream));
        var tags = file.Tag;

        TitleLabel.Content = $"{string.Join(", ", tags.Performers)} - {tags.Title}";

        if (TitleLabel.Content?.ToString() == " - ")
            TitleLabel.Content = files[0].Name;

        if (!OperatingSystem.IsAndroid())
            ((Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!
                .MainWindow as MainWindow)!.SetTitle($"Eclair - {files[0].Name}");

        IPicture? picture = tags.Pictures.Length > 0 ? tags.Pictures[0] : null;

        if (picture != null)
        {
            string fpath = TempPath + $"{tags.Title}-picture0";

            if (File.Exists(fpath))
                goto display;

            var outputstream = File.OpenWrite(fpath);
            outputstream.Write(picture.Data.Data, 0, picture.Data.Count);
            outputstream.Close();

        display:
            MusicPicture.Source = new Bitmap(fpath);
        }
        else MusicPicture.Source = Application.Current?.FindResource("unknowntrack") as Bitmap;

        player.Media = new(vlc, new StreamMediaInput(stream));
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
        MusSlider.Value = 0;
        if (rttransform != null) rttransform.Angle = 0;
        PManager.HidePlayerNotification();

        player.Stop();
    }

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

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        if (e.WidthChanged) MusSlider.Width = (double)(e.NewSize.Width / 1.5);
        Logger.Log($"Size changed: {e.NewSize}");

        base.OnSizeChanged(e);
    }
    private async void SelectFile(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => await GetMusicFile();

    private void GotoSettings(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        prevcontent = Content;
        App.ChangeView(new SettingsView(), this);
    }

    private void PlayButtonSetImage(string imagename)
    {
        if (Application.Current == null) return;
        Dispatcher.UIThread.Invoke(delegate
        {
            PB_Image.Source = (SvgImage?)Application.Current.Resources[imagename + "button"];
        });
    }

    private async void AnimateIcon()
    {
        while (player.IsPlaying)
        {
            try
            {
                await Dispatcher.UIThread.Invoke(async delegate
                {
                    if (rttransform != null)
                        rttransform.Angle += OperatingSystem.IsAndroid() ? 1 : 0.001;
                    if (OperatingSystem.IsAndroid()) await Task.Delay(20);
                });
            }
            catch (TaskCanceledException) { }
        }
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

    private void GotoAbout(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        prevcontent = Content;
        App.ChangeView(new AboutView(), this);
    }

    //                   UseCircleIconAnimation
    internal void Update_UCIA()
    {
        if (Config.UseCircleIconAnimation)
        {
            rttransform = MusicPicture.RenderTransform as RotateTransform;
            MusicPicture.Clip = new EllipseGeometry()
            {
                Center = new(125, 125),
                RadiusX = 125,
                RadiusY = 125
            };
            if (player.IsPlaying)
                Task.Run(AnimateIcon, ctsource.Token);
        }
        else
        {
            rttransform!.Angle = 0;
            MusicPicture.Clip = null;
            rttransform = null;
            if (player.IsPlaying) ctsource.Cancel();
        }
    }
}