using LibVLCSharp.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Eclair;

// playlists will be implemented in the future

// Representing a playlist as a .NET object
public sealed class Playlist
{
    internal List<string> Played = [];
    public string[] Tracks { get; set; } = [];

    public Media PreviousTrack(LibVLC vlc) => new(vlc, Played.Last());

    public Media NextTrack(LibVLC vlc, bool random)
    {
        if (Played.Count == Tracks.Length) throw new Exception("playlist was end");
        if (random)
        {
            Random rand = new();

        random:
            string track = Tracks[rand.Next(Tracks.Length)];
            if (Played.Contains(track)) goto random;

            return new(vlc, track);
        }

        return new(vlc, Tracks[Played.Count]);
    }

    public static Playlist Load(string path)
    {
        try
        {
            return JsonConvert.DeserializeObject<Playlist>(File.ReadAllText(path)) ?? new();
        }
        catch (Exception ex)
        {
            Logger.Log("JsonConvert.DeserializeObject() threw an exception:" + ex.ToString(), Notice);
            return new();
        }
    }
}