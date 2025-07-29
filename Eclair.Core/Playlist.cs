using System.Collections;

namespace Eclair.Core;

public class Playlist(AudioPlayer? player = null) : IPlaylist
{
    int _index = 0;
    protected List<IAudio> List { get; } = [];
    
    public AudioPlayer? Player { get; set; } = player;
    public bool IsAttachedToPlayer => Player != null;

    public IAudio Current => List[Index];

    public IAudio this[int index]
    {
        get => List[index];
        set => List[index] = value;
    }

    public int Index
    {
        get => _index;
        set
        {
            if (value < 0 || value >= Count)
                throw new IndexOutOfRangeException();

            _index = value;
        }
    }
    public int Count => List.Count;
    
    public void Add(IAudio audio) => List.Add(audio);
    public void Remove(IAudio audio) => List.Remove(audio);

    public bool Previous()
    {
        if (Index == 0) return false;
        Index--;
        return true;
    }
    public bool Next()
    {
        if (Index >= List.Count - 1) return false;
        Index++;
        return true;
    }

    public bool PlayPrevious()
    {
        if (!Previous() || Player == null) return false;
        Player.LoadAudio(Current);
        Player.Play();

        return true;
    }
    public bool PlayNext()
    {
        if (!Next() || Player == null) return false;
        Player.LoadAudio(Current);
        Player.Play();
        
        return true;
    }
    
    public IEnumerator<IAudio> GetEnumerator() => List.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();
}