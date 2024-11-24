// todo

using System;

namespace Eclair.Desktop;

internal class PlatformManager : IPlatformManager
{
    public Action? TogglePause { get; set; }
    public Action? Stop { get; set; }

    public void ShowPlayerNotification(string title, bool playing)
    {

    }

    public void HidePlayerNotification() { }
}