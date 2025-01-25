using File = System.IO.File;
using LibVLCSharp.Shared;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Media;
using Avalonia.Svg.Skia;
using Eclair.Controls;

namespace Eclair.Views;

public partial class MainView : UserControl
{
    public const double SnowfallBlurRadius = 5;

    internal static object? prevcontent;

    readonly LibVLC vlc;
    internal MediaPlayer player;
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
                    ciatimer.Stop();
                }
                else if (Config.AutoPlay)
                {
                    if (!PlayNext()) PlayButtonSetImage("play");
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
                    ciatimer.Start();

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

        // if (DateTime.Now.Month is 12 or 1 or 2 && !Config.DisableEffects) // winter
        // {
        //     snowfall = new()
        //     {
        //         Effect = new BlurEffect()
        //         {
        //             Radius = SnowfallBlurRadius
        //         }
        //     };
        //     MainGrid.Children.Insert(0, snowfall);
        // }

        ciatimer.Tick += CIATimer_Tick;

        var args = Environment.GetCommandLineArgs();
        if (args.Length == 2)
        {
            string name = Path.GetFileName(args[1]);
            Stream stream;
            try
            {
                stream = File.OpenRead(args[1]);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return;
            }
            AddMusicItem(name, stream);
            LoadMusicFile(name, stream);
            PlayOrPause();
            ShowPlayer();
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

        if (!OperatingSystem.IsAndroid())
            pathes = [.. pathes, .. Environment.GetLogicalDrives()
                .Where(c => c.StartsWith("/media/") || c.StartsWith("/run/media/")).ToArray()];

        Logger.Log("pathes = " + string.Join(',', pathes));        

        foreach (string path in pathes)
        {
            ScanDirectoryForMusic(path, (mus) =>
                Dispatcher.UIThread.Invoke(() => AddMusicItem(mus))
            );
        }
    }

    #region "Playlist"
    ushort currenttrack = 0;
    internal void PlayTrack(ushort track)
    {
        if (MusicPanel.Children[track] is not Border border) return;
        if (border.Child is not Grid grid) return;
        if (ToolTip.GetTip(grid) is string path)
            LoadMusicFile(Path.GetFileName(path), File.OpenRead(path));
    }
    internal bool PlayNext()
    {
        if (MusicPanel.Children[0] is TextBlock) return false;
        if (currenttrack > MusicPanel.Children.Count) return false;
        ++currenttrack;
        PlayTrack(currenttrack);
        PlayOrPause();
        return true;
    }
    internal void PlayPrevious()
    {
        if (player.Media == null) return;
        if (TimeSpan.FromMilliseconds(
            (double)(player.Media!.Duration * player.Position)).TotalSeconds > 6 ||
            currenttrack == 0)
        {
            player.Position = 0;
            return;
        }
        --currenttrack;
        PlayTrack(currenttrack);
        PlayOrPause();
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
                ciatimer.Start();
        }
        else
        {
            rttransform!.Angle = 0;
            MusicPicture.Clip = null;
            MusicPicture2.Clip = null;
            rttransform = null;
            if (player.IsPlaying) ciatimer.Stop();
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