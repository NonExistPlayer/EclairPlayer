using File = System.IO.File;
using TagFile = TagLib.File;
using TagLib;
using LibVLCSharp.Shared;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Styling;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
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
        Task.Run(FindMusic);
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

    private void FindMusic()
    {
        string[] pathes;

        if (OperatingSystem.IsWindows())
        {
            pathes = [
                $@"{Environment.GetEnvironmentVariable("USERPROFILE")}\Music",
                $@"{Environment.GetEnvironmentVariable("USERPROFILE")}\Documents\Music",
                $@"{Environment.GetEnvironmentVariable("USERPROFILE")}\Desktop",
                $@"{Environment.GetEnvironmentVariable("USERPROFILE")}\Downloads"
            ];
        }
        else if (OperatingSystem.IsAndroid())
        {
            pathes = [
                "/storage/emulated/0/Music",
                "/storage/emulated/0/Downloads",
            ];
        }
        else // linux
        {
            pathes = [
                $@"{Environment.GetEnvironmentVariable("HOME")}/Music",
                $@"{Environment.GetEnvironmentVariable("HOME")}/Downloads",
            ];
        }

        foreach (string path in pathes)
        {
            ScanDirectoryForMusic(path, (mus) =>
                Dispatcher.UIThread.Invoke(() => AddMusicItem(mus))
            );
        }
    }

    internal void AddMusicItem(string path) => AddMusicItem(Path.GetFileName(path), File.OpenRead(path));
    internal void AddMusicItem(string name, Stream stream)
    {
        Logger.Log($"AddMusicItem({name})");
        var tag = TagFile.Create(new ReadOnlyFileImplementation(name, stream)).Tag;

        var border = new Border
        {
            CornerRadius = new CornerRadius(7),
            Margin = new Thickness(5),
            Background = Application.Current?.ActualThemeVariant == ThemeVariant.Light ?
                Brushes.LightGray : Brushes.Gray
        };

        var grid = new Grid { Height = 64 };

        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var trackImage = new Image
        {
            Height = 56,
            Margin = new Thickness(6, 0, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };

        IPicture? picture = tag.Pictures.Length > 0 ? tag.Pictures[0] : null;

        if (picture != null)
        {
            string fpath = TempPath + $"{tag.Title}-picture0";

            if (File.Exists(fpath))
                goto display;

            var outputstream = File.OpenWrite(fpath);
            outputstream.Write(picture.Data.Data, 0, picture.Data.Count);
            outputstream.Close();

        display:
            trackImage.Source = new Bitmap(fpath);
        }
        else trackImage.Source = Application.Current?.FindResource("unknowntrack") as Bitmap;

        var textBlock = new TextBlock
        {
            Text = $"{string.Join(", ", tag.Performers)} - {tag.Title}",
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 0, 0, 0)
        };

        if (textBlock.Text == " - ")
            textBlock.Text = name;

        var playButtonImage = new Image
        {
            Source = (IImage?)Application.Current!.Resources["playbutton"],
            Height = 48
        };

        var button = new Button
        {
            Content = playButtonImage,
            HorizontalAlignment = HorizontalAlignment.Right,
            Background = Brushes.Transparent
        };

        button.Click += delegate
        {
            LoadMusicFile(name, stream);
            PlayOrPause();
            ShowPlayer();
        };

        Grid.SetColumn(textBlock, 1);
        Grid.SetColumn(button, 2);

        grid.Children.Add(trackImage);
        grid.Children.Add(textBlock);
        grid.Children.Add(button);

        border.PointerPressed += (s, e) =>
        {
            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
            LoadMusicFile(name, stream);
            ShowPlayer(s, e);
        };

        border.Child = grid;

        MusicPanel.Children.Add(border);
    }

    internal void LoadMusicFile(string name, Stream stream)
    {
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

        var file = TagFile.Create(new ReadOnlyFileImplementation(name, stream));
        var tags = file.Tag;

        TitleLabel.Content = $"{string.Join(", ", tags.Performers)} - {tags.Title}";

        if (TitleLabel.Content?.ToString() == " - ")
            TitleLabel.Content = name;

        if (!OperatingSystem.IsAndroid())
            ((Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!
                .MainWindow as MainWindow)!.SetTitle($"Eclair - {name}");

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

    #region Events
    private void GotoSettings(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        prevcontent = Content;
        App.ChangeView(new SettingsView(), this);
    }
    private void GotoAbout(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        prevcontent = Content;
        App.ChangeView(new AboutView(), this);
    }
    private void BackToList(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainGrid.RowDefinitions[1] = new(GridLength.Star);
        MainGrid.RowDefinitions[2] = new(new(100));
        MainGrid.RowDefinitions[3] = new(new(0));
        AudioPanel.IsVisible = true;
        BackButton.IsVisible = false;
        BackButton.IsEnabled = false;
    }
    private void ShowPlayer(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        ShowPlayer();
    }
    internal void ShowPlayer()
    {
        MainGrid.RowDefinitions[1] = new(new(0));
        MainGrid.RowDefinitions[2] = new(new(0));
        MainGrid.RowDefinitions[3] = new(GridLength.Star);
        AudioPanel.IsVisible = false;
        BackButton.IsVisible = true;
        BackButton.IsEnabled = true;
    }
    #endregion

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