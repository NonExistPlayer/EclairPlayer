using System;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using ManagedBass;

namespace Eclair.Controls;

public sealed class AudioVisualizer : Control
{
    internal int StreamHandle;

    float[] fftData = new float[256];

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (StreamHandle == 0) return;

        if (Bass.ChannelGetData(StreamHandle, fftData, (int)DataFlags.FFT512) == -1)
        {
            Logger.Warn("Bass.ChannelGetData() returned -1: " + Bass.LastError);

            return;
        }

        // float maxAmplitude = 0;
        // for (int i = 0; i < fftData.Length; i++)
        // {
        //     fftData[i] = MathF.Log10(1 + fftData[i]);
        //     if (fftData[i] > maxAmplitude)
        //     {
        //         maxAmplitude = fftData[i];
        //     }
        // }

        // if (maxAmplitude > 0)
        // {
        //     for (int i = 0; i < fftData.Length; i++)
        //     {
        //         fftData[i] /= maxAmplitude;
        //     }
        // }

        var pen = new Pen(Brushes.LightBlue, 1);
        var height = Bounds.Height / 2;

        for (int i = 0; i < fftData.Length; i++)
        {
            var x = i * 4;
            var y = height * (1 - fftData[i]);
            context.DrawLine(pen, new(x, height), new(x, y));
            context.DrawLine(pen, new(x, height), new(x, Bounds.Height * fftData[i] + height));
        }

        DispatcherTimer.RunOnce(InvalidateVisual, TimeSpan.FromMilliseconds(25));
    }
}