using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Eclair;

#pragma warning disable SYSLIB1054
#pragma warning disable CA1401

[SupportedOSPlatform("windows")]
public static class WinApi
{
    [DllImport("kernel32.dll")]
    public extern static bool AllocConsole();
    [DllImport("kernel32.dll")]
    public static extern bool FreeConsole();
}