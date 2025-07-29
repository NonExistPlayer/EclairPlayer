namespace Eclair.Core;

/// <summary>
/// Represents an audio for <see cref="AudioPlayer"/>.
/// </summary>
public interface IAudio
{
    /// <summary>
    /// The path to the file. May be null.
    /// </summary>
    public string? LocalPath { get; }
    /// <summary>
    /// The audio data (content of the file).<br/>
    /// <b>MUST</b> be with lazy initialization. Especially for using in <see cref="IPlaylist"/>.<br/>
    /// Because it takes place in memory (wow) and if it will be initialized from the constructor,
    /// may be too much memory consumption.
    /// </summary>
    public byte[] AudioData { get; }
    /// <summary>
    /// The file name.
    /// </summary>
    public string FileName { get; }
}