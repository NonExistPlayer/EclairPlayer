// This file handles animations in MainView.
using System;
using Avalonia.Threading;

namespace Eclair.Views;

partial class MainView
{
                    // CIA = Circle Icon Animation
    internal DispatcherTimer ciatimer = new()
    {
        Interval = TimeSpan.FromMilliseconds(40),
    };

    void CIATimer_Tick(object? sender, EventArgs e)
    {
        if (!player.IsPlaying)
        {
            ciatimer.Stop();
            return;
        }

        rttransform!.Angle += 0.5;
    }
}