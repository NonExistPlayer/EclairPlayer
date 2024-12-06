using System;

namespace Eclair;

internal interface IPlatformManager
{
    public static IPlatformManager Null = new NullPlatformManager();
    private class NullPlatformManager : IPlatformManager
    {
        public Action? TogglePause { get; set; }
        public Action? Stop { get; set; }
        public void ShowPlayerNotification(string s, bool b) { }
        public void HidePlayerNotification() { }
    }

    Action? TogglePause { get; set; }
    Action? Stop { get; set; }

    void ShowPlayerNotification(string currentTrackTitle, bool isPlaying);

    void HidePlayerNotification();
}