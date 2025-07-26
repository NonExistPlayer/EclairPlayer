using ManagedBass;

namespace Eclair.Core;

/// <param name="lib">Library name.</param>
/// <param name="ex">Bass exception instance.</param>
public class PluginLoadExceptionEventArgs(string lib, BassException ex) : PluginLoadEventArgs(lib, 0)
{
    public BassException Exception { get; } = ex;
}