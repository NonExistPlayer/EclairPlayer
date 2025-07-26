using ManagedBass;

namespace Eclair.Core;

/// <summary>
/// it's literally an audio player. what did you want to see here? :)
/// </summary>
public class AudioPlayer : IDisposable
{
    #region Properties
    public PlaybackState State => Bass.ChannelIsActive(StreamHandle);
    public bool IsPlaying => State == PlaybackState.Playing;
    public bool IsPaused => State == PlaybackState.Paused;
    /// <summary>
    /// Current music position in seconds.
    /// May return -1 on failure. Check for error via <see cref="Bass.LastError"/>
    /// </summary>
    public double CurrentPositionInSeconds
    {
        get
        {
            if (StreamHandle == 0 || !IsPlaying && !IsPaused) return 0;

            return
                Bass.ChannelBytes2Seconds(StreamHandle, Bass.ChannelGetPosition(StreamHandle));
        }
        set
        {
            if (StreamHandle != 0)
                Bass.ChannelSetPosition(StreamHandle, Bass.ChannelSeconds2Bytes(StreamHandle, value));
        }
    }
    /// <summary>
    /// Current position of music in bytes.
    /// May return -1 on failure. Check for error via <see cref="Bass.LastError"/>
    /// </summary>
    public long CurrentPosition
    {
        get
        {
            if (StreamHandle == 0 || !IsPlaying && !IsPaused) return 0;

            return Bass.ChannelGetPosition(StreamHandle);
        }
        set
        {
            if (StreamHandle != 0)
                Bass.ChannelSetPosition(StreamHandle, value);
        }
    }
    /// <summary>
    /// Length of music in bytes.
    /// May return -1 on failure. Check for error via <see cref="Bass.LastError"/>
    /// </summary>
    public long TrackLength => Bass.ChannelGetLength(StreamHandle);
    /// <summary>
    /// Length of music in seconds.
    /// May return -1 on failure. Check for error via <see cref="Bass.LastError"/>
    /// </summary>
    public double TrackLengthInSeconds => Bass.ChannelBytes2Seconds(StreamHandle, TrackLength);
    /// <summary>
    /// The current audio that is loaded.
    /// May be null if audio is not loaded.
    /// </summary>
    public IAudio? CurrentAudio { get; protected set; }
    public int StreamHandle { get; protected set; }
    #endregion

    #region Private Properties and Fields
    readonly List<int> phnds = []; // PluginsHandles
    #endregion

    #region Events
    /// <summary>
    /// Occurs before audio is loaded.
    /// </summary>
    public event EventHandler<IAudio>? AudioLoading;
    /// <summary>
    /// Occurs after audio is loaded.
    /// </summary>
    public event EventHandler<IAudio>? AudioLoaded;
    /// <summary>
    /// Occurs when <see cref="Play()"/>  is called.
    /// </summary>
    public event EventHandler? AudioPlay;
    //// <summary>
    /// Occurs when <see cref="Pause()"/>  is called.
    /// </summary>
    public event EventHandler? AudioPaused;
    /// <summary>
    /// Occurs when <see cref="Stop()"/>  is called.
    /// </summary>
    public event EventHandler? AudioStop;
    /// <summary>
    /// Occurs when plugin successfully loaded.
    /// </summary>
    public event EventHandler<PluginLoadEventArgs>? PluginLoaded;
    /// <summary>
    /// Occurs when plugin failed to load.
    /// </summary>
    public event EventHandler<PluginLoadExceptionEventArgs>? PluginLoadFailed;
    #endregion

    /// <summary>
    /// yep, it's just a constructor.
    /// </summary>
    /// <param name="initialize">Should be constructor initialize bass.</param>
    /// <exception cref="BassException"/>
    public AudioPlayer(bool initialize = false)
    {
        if (initialize)
        {
            if (!Bass.Init())
            {
                if (Bass.LastError != Errors.Already)
                    throw new BassException();
            }
        }
    }

    /// <summary>
    /// Loads audio.<br/>
    /// Unloads current one if it was loaded.<br/>
    /// Invokes <see cref="AudioLoading"/> at the start and <see cref="AudioLoaded"/> at the end.<br/>
    /// Sets <see cref="StreamHandle"/> and <see cref="CurrentAudio"/>.
    /// </summary>
    /// <param name="audio">The audio for load.</param>
    /// <exception cref="BassException"/>
    public void LoadAudio(IAudio audio)
    {
        AudioLoading?.Invoke(this, audio);

        if (StreamHandle != 0)
            UnloadAudio();

        if ((
            StreamHandle = Bass.CreateStream(audio.AudioData, 0, audio.AudioData.Length, BassFlags.Default
            )) == 0)
        {
            throw new BassException();
        }

        CurrentAudio = audio;

        AudioLoaded?.Invoke(this, audio);
    }

    /// <summary>
    /// Unloads audio.<br/>
    /// Throws <see cref="InvalidOperationException"/> if the audio was not loaded.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void UnloadAudio()
    {
        if (StreamHandle == 0)
            throw new InvalidOperationException("Audio not loaded.");

        Stop();
        Bass.StreamFree(StreamHandle);
        StreamHandle = 0;
        CurrentAudio = null;
    }

    /// <summary>
    /// Loads the bass plugin.<br/>
    /// Invokes <see cref="PluginLoaded"/> if it successfully loaded<br/>
    /// otherwises invokes <see cref="PluginLoadFailed"/> but if its null, throws <see cref="BassException"/>.
    /// </summary>
    /// <param name="lib"></param>
    /// <exception cref="BassException"/>
    public void PluginLoad(string lib)
    {
        if (OperatingSystem.IsWindows())
            lib = $"bass{lib}.dll";
        else // linux / android
            lib = $"libbass{lib}.so";

        int hnd = Bass.PluginLoad(lib);

        if (hnd == 0)
        {
            var exception = new BassException();
            if (PluginLoadFailed != null)
                PluginLoadFailed(this, new PluginLoadExceptionEventArgs(lib, exception));
            else
                throw exception;
        }
        else
            PluginLoaded?.Invoke(this, new PluginLoadEventArgs(lib, hnd));

        phnds.Add(hnd);
    }

    #region Audio Control
    /// <summary>
    /// Plays the audio.<br/>
    /// Invokes <see cref="AudioPlay"/>.<br/>
    /// Does nothing if there is no audio.
    /// </summary>
    /// <exception cref="BassException"/>
    public void Play()
    {
        if (StreamHandle == 0) return;

        if (!Bass.ChannelPlay(StreamHandle))
            throw new BassException();

        AudioPlay?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// Pauses the audio.<br/>
    /// Invokes <see cref="AudioPaused"/>.<br/>
    /// Does nothing if there is no audio.
    /// </summary>
    /// <exception cref="BassException"/>
    public void Pause()
    {
        if (StreamHandle == 0) return;

        if (!Bass.ChannelPause(StreamHandle))
            throw new BassException();

        AudioPaused?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// Stops the audio.<br/>
    /// Invokes <see cref="AudioStop"/>.<br/>
    /// yeah. it still does nothing if there is no audio.
    /// </summary>
    /// <exception cref="BassException"/>
    public void Stop()
    {
        if (StreamHandle == 0) return;

        AudioStop?.Invoke(this, EventArgs.Empty);

        if (!Bass.ChannelStop(StreamHandle))
            throw new BassException();
    }
    #endregion

    #region IDisposable
    private bool disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;

        if (disposing)
        {
            if (StreamHandle != 0)
            {
                Bass.StreamFree(StreamHandle);
                StreamHandle = 0;
            }
            Bass.Free();
            foreach (int hnd in phnds)
                Bass.PluginFree(hnd);
            disposed = true;
        }
    }

    /// <summary>
    /// wow. it's just disposing. amazing.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~AudioPlayer()
    {
        Dispose(false);
    }
    #endregion
}