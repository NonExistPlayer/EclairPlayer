namespace Eclair.Core;

/// <param name="lib">Library name.</param>
/// <param name="hnd">Library bass handle.</param>
public class PluginLoadEventArgs(string lib, int hnd)
{
    public string Library { get; } = lib;
    public int Handle { get; } = hnd;
}