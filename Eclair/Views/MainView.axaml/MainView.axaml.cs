using File = System.IO.File;
using ManagedBass;
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
using System.Collections.Generic;

namespace Eclair.Views;

public partial class MainView : UserControl, IDisposable
{
    public const double SnowfallBlurRadius = 5;

    internal static object? prevcontent;

    bool calledByPlayer;
    Control[]? musicitems;

    bool loop = false;

    RotateTransform? rttransform;

    Snowfall? snowfall;

    public MainView()
    {
        InitializeComponent();
        Task.Run(FindMusic);
        
        if (!Bass.Init())
        {
            Logger.Log("Failed to initialize BASS!", Critical);
            Environment.Exit(1);
        }

        #region Plugin Load
        BassPluginLoad("flac");
        BassPluginLoad("midi");
        BassPluginLoad("wv");
        BassPluginLoad("opus");
        BassPluginLoad("dsd");
        BassPluginLoad("alac");
        BassPluginLoad("webm");
        BassPluginLoad("aac");
        #endregion

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

        timer.Tick += Timer_Tick;

        var args = Environment.GetCommandLineArgs();
        if (args.Length == 2)
        {
            PlayTrack(AddMusicItem(new(args[1])));
            PlayOrPause();
            ShowPlayer();
        }
        MusSlider.PointerReleased += SliderPointerReleased;

        MusSlider.AddHandler(PointerReleasedEvent, SliderPointerReleased, Avalonia.Interactivity.RoutingStrategies.Tunnel);

        if (Config.EnableVisualizer)
            Visualizer.Effect = new BlurEffect();
        else
            Visualizer.IsVisible = false;
    }

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
            ScanDirectoryForMusic(path, (path) =>
                Dispatcher.UIThread.Invoke(() => AddMusicItem(new(path)))
            );
        }
    }

    #region "Playlist"
    List<Media> playlist = [];
    ushort currenttrack = 0;
    internal void LoadTrack(ushort track)
    {
        LoadMusicFile(playlist[track]);
    }
    internal void PlayTrack(ushort track)
    {
        LoadTrack(track);
        PlayOrPause();
    }
    internal bool PlayNext()
    {
        if (MusicPanel.Children[0] is TextBlock) return false;
        if (currenttrack + 1 == playlist.Count) return false;
        ++currenttrack;
        PlayTrack(currenttrack);
        return true;
    }
    internal void PlayPrevious()
    {
        if (shnd == 0) return;
        if (TimeSpan.FromSeconds(
            CurrentPos).TotalSeconds > 6 ||
            currenttrack == 0)
        {
            CurrentPos = 0;
            return;
        }
        --currenttrack;
        PlayTrack(currenttrack);
    }
    #endregion

    #region Overrides
    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            MusSlider.Width = Math.Min(e.NewSize.Width / 1.5, 902);
            SearchBox.Width = (double)(e.NewSize.Width / 1.5);
            Visualizer.Width = e.NewSize.Width / (OperatingSystem.IsAndroid() ? 1.1 : 2);
            snowfall?.WidthChanged((int)e.NewSize.Width);
        }
        else if (e.HeightChanged)
        {
            if (!OperatingSystem.IsAndroid() && snowfall != null)
                snowfall.Height = (int)e.NewSize.Height;
        }

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
            if (isplaying)
                timer.Start();
        }
        else
        {
            rttransform!.Angle = 0;
            MusicPicture.Clip = null;
            MusicPicture2.Clip = null;
            rttransform = null;
            if (isplaying) timer.Stop();
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
    internal void Update_Visualizer() =>
        Visualizer.IsVisible = Config.EnableVisualizer;
    #endregion
}