global using static Eclair.EclairLogLevel;
using NonExistPlayer.Logging;
using System;

namespace Eclair;

public class EclairLogLevel : ILogLevel
{
    public EclairLogLevel(ushort value)
    {
        if (value > Max) throw new ArgumentOutOfRangeException(nameof(value));
        Value = value;
    }

    public static ushort Max => 6;
    public static ushort Min => 0;

    public static EclairLogLevel Default { get; } = new(0);
    public static EclairLogLevel Notice { get; } = new(2);
    public static EclairLogLevel Critical { get; } = new(5);

    public ushort Value { get; }

    public override string ToString()
    {
        return Value switch
        {
            0 => "Debug",
            1 => "Info",
            2 => "Notice",
            3 => "Warning",
            4 => "Error",
            5 => "Critical",
            _ => ""
        };
    }

    public ConsoleColor GetColor()
    {
        return Value switch
        {
            0 => ConsoleColor.White,
            2 => ConsoleColor.Blue,
            3 => ConsoleColor.Yellow,
            4 => ConsoleColor.Red,
            5 => ConsoleColor.DarkRed,
            _ => ConsoleColor.Gray
        };
    }

    public bool IsError() => Value is 4 or 5;
}