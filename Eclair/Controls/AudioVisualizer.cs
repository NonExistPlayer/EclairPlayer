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

        double step = Bounds.Width / fftData.Length;
        var pen = new Pen(new SolidColorBrush(Config.VisualizerColor), 1);
        var height = Bounds.Height / 2;
        
        var totalWidth = fftData.Length * step;
        var centerX = Bounds.Width / 2;
        var startX = centerX - totalWidth / 2;

        for (int i = 0; i < fftData.Length; i++)
        {
            var x = startX + i * step;

            var yTop = height * (1 - fftData[i]);
            var yBottom = height * (1 + fftData[i]);

            context.DrawLine(pen, new(x, height), new(x, yTop));
            context.DrawLine(pen, new(x, height), new(x, yBottom));
        }

        DispatcherTimer.RunOnce(InvalidateVisual, TimeSpan.FromMilliseconds(25));
    }
}