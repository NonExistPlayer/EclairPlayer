namespace Eclair.Core;

public interface IAudio
{
    public string? LocalPath { get; }
    public byte[] AudioData { get; }
    public string FileName { get; }
}