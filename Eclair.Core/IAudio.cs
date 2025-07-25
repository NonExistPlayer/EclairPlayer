using TagLib;

namespace Eclair.Core;

public interface IAudio
{
    public string? LocalPath { get; }
    public byte[] AudioData { get; }
    public string FileName { get; }
    public Tag Tags { get; }

    public string FullTitle { get; }
    public string Title { get; }
    public string Performers { get; }
}