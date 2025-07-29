using TagLib;

namespace Eclair.Core;

/// <summary>
/// Represents an audio as a song. With some audio metadata.
/// </summary>
public interface ISong : IAudio
{
    public string Title { get; }
    public string Performers { get; }
    public string Album { get; }
    public Tag Tags { get; }
}