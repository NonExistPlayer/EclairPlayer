// buildcore.csx created by NonExistPlayer for EclairPlayer (https://github.com/NonExistPlayer/EclairPlayer)

using System;
using System.Diagnostics;
using static System.Console;

static int Run(string command, bool visible = true) {
    var _ = command.Split(' ');
    string name = _[0];
    string args = string.Join(' ', _[1..]);
    var proc = new Process()
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = name,
            Arguments = args
        }
    };

    if (!proc.Start()) return int.MinValue;

    if (visible)
        WriteLine(name + " " + args);
    
    proc.WaitForExit();

    if (proc.ExitCode != 0)
        WriteLine($"{name} exited with code: {proc.ExitCode}");

    return proc.ExitCode;
}

static bool BuildForPlatform(string platform, string config, string arch, bool ispublish = true) {
    return Run("dotnet " +
@$"../Eclair.{(platform != "android" ? "Desktop" : "Android")}
--no-restore -o ../{(ispublish ? "publish" : "output")} -c {config} --nologo
--os {platform}")
        == 0;
}

static string Architecture =
    System.
    Runtime.
    InteropServices.
    RuntimeInformation.
    ProcessArchitecture.
    ToString().
    ToLower();