using System;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Eclair.Android")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("eclairplayer")]

namespace Eclair;

internal interface IPlatformManager
{
    public static IPlatformManager Null = new NullPlatformManager();
    private class NullPlatformManager : IPlatformManager
    {
        public Action? TogglePause { get; set; }
        public void ShowPlayerNotification(string s, bool b) { }
        public void HidePlayerNotification() { }
    }

    Action? TogglePause { get; set; }

    void ShowPlayerNotification(string currentTrackTitle, bool isPlaying);

    void HidePlayerNotification();
}