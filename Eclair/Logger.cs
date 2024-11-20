﻿global using static LibVLCSharp.Shared.LogLevel;
using LibVLCSharp.Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Eclair;

internal static class Logger
{
    private static object _lock = new();
    public static bool Init(TextWriter? file)
    {
        try
        {
            FileStream = file;
            if (!OperatingSystem.IsAndroid()) Console.Title = "Eclair Dev Console";
            WriteLine("Eclair.Logger was initialized.");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static TextWriter? FileStream { get; private set; }

    private static ConsoleColor GetColorByLLevel(LogLevel t)
    {
        return t switch
        {
            Notice => ConsoleColor.Blue,
            Warning => ConsoleColor.Yellow,
            Error => ConsoleColor.Red,
            _ => ConsoleColor.Gray
        };
    }

    public static void WriteLine(string message, LogLevel type = LogLevel.Debug, string? callerstr = null)
    {
        lock (_lock)
        {
            if (!OperatingSystem.IsAndroid()) Console.ForegroundColor = GetColorByLLevel(type);

            MethodBase? caller = new StackFrame(1).GetMethod();

            if (caller != null && callerstr == null)
            {
                if (caller.DeclaringType != null)
                    callerstr = caller.DeclaringType.FullName + "." + caller.Name;
            }

            message = $"[{DateTime.Now:HH:mm:ss}] ({callerstr ?? "<unknown>"}) {type}: {message}";

            FileStream?.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
            if (!OperatingSystem.IsAndroid())
            {
                if (type != Error)
                    Console.WriteLine(message);
                else
                    Console.Error.WriteLine(message);
            }

            if (!OperatingSystem.IsAndroid()) Console.ResetColor();
        }
    }
}