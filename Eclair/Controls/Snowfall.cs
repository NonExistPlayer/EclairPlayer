using System;
using System.Timers;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Eclair.Controls;

public sealed class Snowfall : Control
{
    public class Snowflake
    {
        public const double Speed = 0.9;
        public const byte TransparentLevel = 165;

        public double X, Y;
        public double Size;
    }

    private Snowflake Generate()
    {
        return new Snowflake
        {
            X = rnd.Next(0, Width),
            Y = -64,
            Size = rnd.Next(16, 52)
        };
    }

    new public int Width { get; private set; }
    new public int Height { get; internal set; } = 2000;

    readonly Timer updatetimer;
    readonly Timer addtimer;
    List<Snowflake>? snowflakes;
    readonly Random rnd = new();

    public Snowfall()
    {
        updatetimer = new()
        {
            Interval = 16,
            AutoReset = true
        };
        addtimer = new()
        {
            Interval = rnd.Next(400, 500),
            AutoReset = true
        };

        updatetimer.Elapsed += Update;
        updatetimer.Start();
        addtimer.Elapsed += (s, e) => { snowflakes?.Add(Generate()); };
        addtimer.Start();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (snowflakes == null) return;

        for (int i = 0; i < snowflakes.Count; i++)
        {
            var snowflake = snowflakes[i];
            context.DrawEllipse(
                new SolidColorBrush(
                    new Color(Snowflake.TransparentLevel, 255, 255, 255)),
                null,
                new Rect(snowflake.X, snowflake.Y, snowflake.Size, snowflake.Size)
            );
        }
    }

    private void Update(object? s, EventArgs e)
    {
        foreach (var snowflake in snowflakes ?? [])
        {
            snowflake.Y += Snowflake.Speed;
            if (snowflake.Y > Height)
                snowflakes?.Remove(snowflake);
        }
        Avalonia.Threading.Dispatcher.UIThread.Invoke(InvalidateVisual);
    }

    public void WidthChanged(int newwidth)
    {
        Width = newwidth;
        snowflakes ??= [ Generate() ];
    }
}