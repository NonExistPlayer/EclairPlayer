// This file handles animations in MainView.
using System;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Eclair.Views;

partial class MainView
{
                    // CIA = Circle Icon Animation
    internal DispatcherTimer timer = new()
    {
        Interval = TimeSpan.FromMilliseconds(40),
    };

    void Timer_Tick(object? sender, EventArgs e)
    {
        if (!isplaying)
        {
            timer.Stop();
            return;
        }

        if (rttransform != null)
            rttransform.Angle += 0.5;
        
        if (MainGrid.RowDefinitions[3].Height == GridLength.Star /*if fullscreen player*/ &&
            !sliderPressed)
        {
            Dispatcher.UIThread.InvokeAsync(delegate
            {
                calledByPlayer = true;
                MusSlider.Value = CurrentPos;
            });
        }
    }
}