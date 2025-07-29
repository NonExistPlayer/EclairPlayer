namespace Eclair.Core;

public interface IPlaylist : IEnumerable<IAudio>
{
    /// <summary>
    /// The audio player.
    /// </summary>
    AudioPlayer? Player { get; set; }
    /// <summary>
    /// Indicates whether this playlist is attached to the player.
    /// </summary>
    bool IsAttachedToPlayer { get; }
    
    /// <summary>
    /// The current audio.
    /// </summary>
    IAudio Current { get; }
    IAudio this[int index] { get; }
    
    /// <summary>
    /// Index of current item.
    /// </summary>
    int Index { get; set; }
    /// <summary>
    /// Total count of items.
    /// </summary>
    int Count { get; }
    
    /// <summary>
    /// Adds audio to playlist.
    /// </summary>
    /// <param name="audio">The audio.</param>
    void Add(IAudio audio);
    /// <summary>
    /// Removes the audio from playlist.
    /// </summary>
    /// <param name="audio">The audio to remove.</param>
    void Remove(IAudio audio);
    
    /// <summary>
    /// Sets current item of playlist to previous audio.
    /// </summary>
    /// <returns>Is successfully.</returns>
    bool Previous();
    /// <summary>
    /// Sets current item of playlist to next audio.
    /// </summary>
    /// <returns>Is successfully.</returns>
    bool Next();
    
    /// <summary>
    /// Plays previous audio.
    /// </summary>
    /// <returns>Is successfully.</returns>
    bool PlayPrevious();
    /// <summary>
    /// Plays next audio.
    /// </summary>
    /// <returns>Is successfully.</returns>
    bool PlayNext();
}