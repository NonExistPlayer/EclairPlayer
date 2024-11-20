using Avalonia;
using Avalonia.Media;
using System;

namespace Eclair;

public class CircularWaveformVisualizer : Visual
{
    private float[] aData;

    public CircularWaveformVisualizer() => aData = [];
#pragma warning disable CS8618
    public CircularWaveformVisualizer(float[] aData) => UpdateWaveform(aData);
#pragma warning restore CS8618

    public void UpdateWaveform(float[] aData)
    {
        this.aData = aData;
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (aData.Length == 0) return;

        double width, height, centerx, centery, radius;

        width = Bounds.Width;
        height = Bounds.Height;
        centerx = width / 2;
        centery = height / 2;
        radius = Math.Min(centerx, centery) * 0.8;

        var pen = new Pen(Brushes.Blue, 2);

        for (int i = 1; i < aData.Length; i++)
        {
            double angle = i / (double)aData.Length * 2 * Math.PI;
            double sampleValue = aData[i] * radius;

            double x = centerx + Math.Cos(angle) * sampleValue;
            double y = centery + Math.Sin(angle) * sampleValue;

            double prevAngle = (i - 1) / (double)aData.Length * 2 * Math.PI;
            double prevX = centerx + Math.Cos(prevAngle) * aData[i - 1] * radius;
            double prevY = centery + Math.Sin(prevAngle) * aData[i - 1] * radius;

            context.DrawLine(pen, new Point(prevX, prevY), new Point(x, y));
        }

        context.Dispose();
    }
}