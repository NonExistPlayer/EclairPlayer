using File = System.IO.File;
using TagFile = TagLib.File;
using TagLib;
using LibVLCSharp.Shared;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Styling;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Svg.Skia;
using Eclair.Controls;

namespace Eclair.Views;

public partial class MainView : UserControl
{
    internal static object? prevcontent;

    readonly LibVLC vlc;
    internal MediaPlayer player;
    readonly CancellationTokenSource ctsource = new();
    bool calledByPlayer;
    Control[]? musicitems;

    bool loop = false;

    RotateTransform? rttransform;

    Snowfall? snowfall;

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

                if (PManager != null && !loop)
                    PManager.ShowPlayerNotification(TitleLabel.Content?.ToString()!, false);
            });
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
        {
            rttransform = MusicPicture.RenderTransform as RotateTransform;
            MusicPicture2.RenderTransform = rttransform;
        }
        else
        {
            MusicPicture.Clip = null;
            MusicPicture2.Clip = null;
        }

        PManager.TogglePause += PlayOrPause;
        PManager.Stop += Stop;

        AudioPanel.Background = new SolidColorBrush(new Color(125, 212, 212, 212));

        if (OperatingSystem.IsAndroid())
            TitleText.MaxWidth = 128;

        if (DateTime.Now.Month is 12 or 1 or 2 && !Config.DisableEffects) // winter
        {
            snowfall = new()
            {
                Effect = new BlurEffect()
                {
                    Radius = 0.25
                }
            };
            MainGrid.Children.Insert(0, snowfall);
        }
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
                "/storage/emulated/0/Download",
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

    internal void AddMusicItem(string path)
    {
        string name = Path.GetFileName(path);
        Stream stream = File.OpenRead(path);
        Logger.Log($"AddMusicItem({path})");
        if (MusicPanel.Children.Count == 1 &&
            MusicPanel.Children[0] is TextBlock)
            MusicPanel.Children.Clear();
        var tag = TagFile.Create(new ReadOnlyFileImplementation(name, stream)).Tag;

        var border = new Border
        {
            CornerRadius = new CornerRadius(7),
            Margin = new Thickness(5),
            Background = Application.Current?.ActualThemeVariant == ThemeVariant.Light ?
                new SolidColorBrush(new Color(125, 199, 199, 199)) : new SolidColorBrush(new Color(125, 133, 133, 133))
        };

        var grid = new Grid { Height = 64 };
        ToolTip.SetTip(grid, path);

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
        };

        Grid.SetColumn(textBlock, 1);
        Grid.SetColumn(button, 2);

        grid.Children.Add(trackImage);
        grid.Children.Add(textBlock);
        grid.Children.Add(button);

        border.PointerReleased += (s, e) =>
        {
            if (e.InitialPressMouseButton == Avalonia.Input.MouseButton.Right /* <-- i have no idea why it works like this*/) return;
            LoadMusicFile(name, stream);
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

        SetTitle($"{string.Join(", ", tags.Performers)} - {tags.Title}");

        if (TitleLabel.Content?.ToString() == " - ")
            SetTitle(name);

        SetTitle(TitleLabel.Content?.ToString());

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
            SetImage(new Bitmap(fpath));
        }
        else SetImage(Application.Current?.FindResource("unknowntrack") as Bitmap);

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
    private async void SelectFile(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => await GetMusicFile();
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
        SearchBox.IsEnabled = true;
        BackButton.IsVisible = false;
        BackButton.IsEnabled = false;
    }
    private void ShowPlayer(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == Avalonia.Input.MouseButton.Right) return;
        ShowPlayer();
    }
    private void TextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (SearchBox.Text == "")
        {
            MusicPanel.Children.Clear();
            MusicPanel.Children.AddRange(musicitems ?? []);
            musicitems = null;
            return;
        }

        if (musicitems is null)
        {
            musicitems = new Control[MusicPanel.Children.Count];
            MusicPanel.Children.CopyTo(musicitems, 0);
        }

        if (MusicPanel.Children.Count == 1 &&
            MusicPanel.Children[0] is TextBlock)
            MusicPanel.Children.Clear();

        MusicPanel.Children.Clear();

        MusicPanel.Children.AddRange(
            musicitems.Where(m =>
                ((TextBlock)
                    ((Grid)
                        ((Border)m).Child!)
                            .Children[1])
                            .Text!
                            .Contains(SearchBox.Text ?? "", StringComparison.CurrentCultureIgnoreCase)
            )
        );

        if (MusicPanel.Children.Count == 0)
            MusicPanel.Children.Add(new TextBlock
            {
                Text = resources.ui_nothingfound + "...",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeight.Bold,
                FontSize = 24
            });
    }
    internal void ShowPlayer()
    {
        MainGrid.RowDefinitions[1] = new(new(0));
        MainGrid.RowDefinitions[2] = new(new(0));
        MainGrid.RowDefinitions[3] = new(GridLength.Star);
        AudioPanel.IsVisible = false;
        SearchBox.IsEnabled = false;
        BackButton.IsVisible = true;
        BackButton.IsEnabled = true;
    }
    #endregion

    #region Overrides
    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            MusSlider.Width = Math.Min(e.NewSize.Width / 1.5, 902);
            SearchBox.Width = (double)(e.NewSize.Width / 1.5);
            snowfall?.WidthChanged((int)e.NewSize.Width);
        }
        else if
            (e.HeightChanged &&
            !OperatingSystem.IsAndroid() &&
            snowfall != null)
            snowfall.Height = (int)e.NewSize.Height;
        Logger.Log($"Size changed: {e.NewSize}");

        base.OnSizeChanged(e);
    }
    #endregion

    #region Set
    internal void SetTitle(string? text)
    {
        TitleLabel.Content = text;
        TitleText.Text = text;
    }
    internal void SetImage(IImage? image)
    {
        MusicPicture.Source = image;
        MusicPicture2.Source = image;
    }
    internal void PlayButtonSetImage(string imagename)
    {
        if (Application.Current == null) return;
        Dispatcher.UIThread.Invoke(delegate
        {
            var image = (SvgImage?)Application.Current.Resources[imagename + "button"];
            PB_Image.Source = image;
            PB2_Image.Source = image;
        });
    }
    #endregion

    #region Settings Updates
    //                   UseCircleIconAnimation
    internal void Update_UCIA()
    {
        if (Config.UseCircleIconAnimation)
        {
            rttransform = MusicPicture.RenderTransform as RotateTransform;
            MusicPicture.Clip = new EllipseGeometry
            {
                Center = new(125, 125),
                RadiusX = 125,
                RadiusY = 125
            };
            MusicPicture2.Clip = new EllipseGeometry
            {
                Center = new(32, 32),
                RadiusX = 32,
                RadiusY = 32
            };
            if (player.IsPlaying)
                Task.Run(AnimateIcon, ctsource.Token);
        }
        else
        {
            rttransform!.Angle = 0;
            MusicPicture.Clip = null;
            MusicPicture2.Clip = null;
            rttransform = null;
            if (player.IsPlaying) ctsource.Cancel();
        }
    }
    //                   DisableEffects
    internal void Update_DEff()
    {
        if (Config.DisableEffects)
        {
            snowfall = null;
            MainGrid.Children.RemoveAt(0);
        }
        else if (DateTime.Now.Month is 12 or 1 or 2)
        {
            snowfall = new()
            {
                Effect = new BlurEffect()
                {
                    Radius = 0.25
                },
                Height = (int)Bounds.Height
            };
            snowfall.WidthChanged((int)Bounds.Width);
            MainGrid.Children.Insert(0, snowfall);
        }
    }
    #endregion
}