using Avalonia;
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

namespace Eclair.Views;

public partial class MainView : UserControl
{
    readonly LibVLC vlc;
    readonly MediaPlayer player;
    bool calledByPlayer;

    bool loop;

    readonly RotateTransform? rttransform;

    public MainView()
    {
        InitializeComponent();
        vlc = new();
        vlc.Log += (s, e) =>
        {
            Logger.WriteLine(e.Message, e.Level, "<LibVLC>");
        };

        player = new(vlc);

        if (App.Config.UseCircleIconAnimation)
            rttransform = MusicPicture.RenderTransform as RotateTransform;
        else
            MusicPicture.Clip = null;

        
    }

    private async void PlayButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (player.Media == null) await GetMusicFile();
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

        if (player.Media == null)
        {
            player.EndReached += delegate
            {
                Dispatcher.UIThread.InvokeAsync(delegate
                {
                    MusSlider.Value = 0;
                    PlayButtonSetImage("play");
                    if (rttransform != null)
                        rttransform.Angle = 0;
                });
            };
            player.PositionChanged += Player_PositionChanged;

            StopButton.IsEnabled = true;
            MusSlider.IsEnabled = true;
            LoopButton.IsEnabled = true;
        }
        else
        {
            player.Stop();
            MusSlider.Value = 0;
            rttransform!.Angle = 0;
            player.Media.Dispose();
        }

        Stream stream = await files[0].OpenReadAsync();

        var file = TagFile.Create(new ReadOnlyFileImplementation(files[0].Name, stream));
        var tags = file.Tag;

        TitleLabel.Content = $"{string.Join(", ", tags.Performers)} - {tags.Title}";

        if (TitleLabel.Content?.ToString() == " - ")
            TitleLabel.Content = files[0].Name;

        if (!OperatingSystem.IsAndroid())
            (Parent as MainWindow)!.Title = $"Eclair - {files[0].Name}";

        IPicture? picture = tags.Pictures.Length > 0 ? tags.Pictures[0] : null;

        if (picture != null)
        {
            string fpath = App.TempPath + $"{tags.Title}-picture0";

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
            player.Pause();
        else
        {
            if (player.Play()
                && App.Config.UseCircleIconAnimation)
                Task.Run(AnimateIcon);
        }
    }

    private void Stop()
    {
        PlayButtonSetImage("play");
        MusSlider.Value = 0;

        player.Stop();
    }

    private void SliderValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (calledByPlayer)
        {
            calledByPlayer = false;
            return;
        }
        player.Position = (float)(e.NewValue / 100);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        if (e.WidthChanged) MusSlider.MaxWidth = (double)(e.NewSize.Width / 1.5);

        base.OnSizeChanged(e);
    }
    private async void SelectFile(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => await GetMusicFile();

    private void GotoSettings(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        (Parent as MainWindow)?.ShowSettings();
    }

    private void PlayButtonSetImage(string imagename)
    {
        if (Application.Current == null) return;
        PB_Image.Source = (SvgImage?)Application.Current.Resources[imagename + "button"];
    }

    private async void AnimateIcon()
    {
        if (rttransform == null) return;

        if (!player.IsPlaying)
        {
            Logger.WriteLine("waiting for LibVLC#...");
            for (byte i = 1; i <= 25; i++) // waiting for LibVLC# to start playing
            {
                Logger.Write($"attempt {i}... ");
                if (player.IsPlaying)
                {
                    Logger.WriteLine("success");
                    break;
                }
                Logger.WriteLine("failed", i >= 10 ? Error : Notice);
                Thread.Sleep(100);
            }
            if (!player.IsPlaying)
            {
                Logger.WriteLine("All attempts are wasted. Failed to animate music icon.", Notice);
                return;
            }
        }

        while (player.IsPlaying)
        {
            await Dispatcher.UIThread.Invoke(async delegate
            {
                rttransform.Angle += OperatingSystem.IsAndroid() ? 1 : 0.001;
                if (OperatingSystem.IsAndroid()) await Task.Delay(20);
            });
        }
    }

    private void LB_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => loop = !loop;
}