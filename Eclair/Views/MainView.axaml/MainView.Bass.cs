// This file is responsible for managing the Bass audio library.

using File = System.IO.File;
using TagFile = TagLib.File;
using TagLib;
using ManagedBass;
using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace Eclair.Views;

partial class MainView
{
    int shnd; // stream handle

    internal bool isplaying;
    internal bool ispaused;

    double CurrentPos
    {
        get 
        {
            if (shnd == 0 || !isplaying && !ispaused) return 0;

            return 
                Bass.ChannelBytes2Seconds(shnd, Bass.ChannelGetPosition(shnd));
        }
        set
        {
            if (shnd != 0)
                Bass.ChannelSetPosition(shnd, Bass.ChannelSeconds2Bytes(shnd, value));
        }
    }

    double tracklength;

    internal bool LoadMusicFile(Media media)
    {
        MusDurationLabel.Content = "00:00";

        if (shnd == 0)
        {
            MusSlider.IsEnabled = true;
        }
        else
        {
            Stop();
            Bass.StreamFree(shnd);
            shnd = 0;
        }

        var tags = media.Tags;

        SetTitle($"{string.Join(", ", tags.Performers)} - {tags.Title}");

        if (TitleLabel.Content?.ToString() == " - ")
            SetTitle(media.FileName);

        SetTitle(TitleLabel.Content?.ToString());

        if (!OperatingSystem.IsAndroid())
            ((Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?
                .MainWindow as MainWindow)?.SetTitle($"Eclair - {media.FileName}");

        SetImage(media.Image ?? Application.Current?.FindResource("unknowntrack") as Bitmap);

        if ((
            shnd = Bass.CreateStream(media.AudioData, 0, media.AudioData.Length, BassFlags.Default
            )) == 0)
        {
            Logger.Error("Failed to load audio! " + Bass.LastError);

            return false;
        }

        Bass.ChannelSetSync(shnd, SyncFlags.End, 0, OnPlaybackFinished);

        MusSlider.Maximum =
            tracklength = Bass.ChannelBytes2Seconds(shnd, Bass.ChannelGetLength(shnd));
        
        Dispatcher.UIThread.Invoke(delegate
        {
            timer.Start();

            MusDurationLabel.Content = TimeSpan.FromSeconds(tracklength).ToString(@"mm\:ss");            
        });

        Visualizer.StreamHandle = shnd;

        return true;
    }

    #region Events
    void OnPlaybackFinished(int handle, int channel, int data, IntPtr user)
    {
        Dispatcher.UIThread.InvokeAsync(delegate
        {
            MusSlider.Value = 0;
            if (loop)
            {
                Stop();
                PlayOrPause();
            }
            else if (Config.AutoPlay)
            {
                if (!PlayNext())
                {
                    Stop();
                    timer.Stop();
                }
            }
            if (PManager != null && !loop)
                PManager.ShowPlayerNotification(TitleLabel.Content?.ToString()!, false);
        });
        sliderPressed = false;
    }
    #endregion

    #region Music Control
    public void Play()
    {
        sliderPressed = false;
        if (shnd == 0) return;

        if (Bass.ChannelPlay(shnd))
        {
            isplaying = true;
            ispaused = false;
        }
    }

    public void Pause()
    {
        if (shnd == 0) return;

        if (Bass.ChannelPause(shnd))
        {
            isplaying = false;
            ispaused = true;
        }
    }

    public void Stop()
    {
        if (shnd == 0) return;

        PlayButtonSetImage("play");
        Dispatcher.UIThread.Invoke(delegate
        {
            MusDurationLabel.Content = "00:00";
            MusPositionLabel.Content = "00:00";
            calledByPlayer = true;
            MusSlider.Value = 0;
            if (rttransform != null) rttransform.Angle = 0;
        });
        PManager.HidePlayerNotification();

        if (Bass.ChannelStop(shnd))
        {
            isplaying = false;
            ispaused = false;
        }
    }
    #endregion

    private bool disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;

        if (disposing)
        {
            if (shnd != 0)
            {
                Bass.StreamFree(shnd);
                shnd = 0;
            }
            Bass.Free();
            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~MainView()
    {
        Dispose(false);
    }
}