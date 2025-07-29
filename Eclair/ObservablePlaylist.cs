using System.Collections.ObjectModel;
using Eclair.Core;

namespace Eclair;

public sealed class ObservablePlaylist(Playlist? playlist = null) : ObservableCollection<IAudio>, IPlaylist
{
    readonly Playlist _playlist = playlist ?? [];

    public AudioPlayer? Player
    {
        get => _playlist.Player;
        set => _playlist.Player = value;
    }
    public bool IsAttachedToPlayer => _playlist.IsAttachedToPlayer;
    public IAudio Current => _playlist.Current;

    public int Index
    {
        get => _playlist.Index;
        set => _playlist.Index = value;
    }

    public new void Add(IAudio audio)
    {
        base.Add(audio);
        _playlist.Add(audio);
    }

    public new void Remove(IAudio audio)
    {
        base.Remove(audio);
        _playlist.Remove(audio);
    }

    public bool Previous() => _playlist.Previous();
    public bool Next() => _playlist.Next();

    public bool PlayPrevious() => _playlist.PlayPrevious();
    public bool PlayNext() => _playlist.PlayNext();
}