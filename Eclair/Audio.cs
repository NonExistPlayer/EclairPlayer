using TagLib;
using System.IO;
using Avalonia.Platform.Storage;

using File = System.IO.File;
using TagFile = TagLib.File;

namespace Eclair;

public class Audio : Core.IAudio
{
    public Audio(IStorageFile file, Stream stream)
    {
        LocalPath = file.TryGetLocalPath();

        var reader = new BinaryReader(stream);
        AudioData = new byte[stream.Length];
        reader.Read(AudioData, 0, AudioData.Length);

        FileName = file.Name;
        Tags = TagFile.Create(new StorageFileAbstraction(file.Name, stream)).Tag;

        Title = Tags.Title;
        Performers = string.Join(", ", Tags.Performers);
        FullTitle = $"{Performers} - {Title}";
    }

    public Audio(string path)
    {
        LocalPath = path;
        AudioData = File.ReadAllBytes(path);
        FileName = Path.GetFileName(path);
        Tags = TagFile.Create(path).Tag;

        Title = Tags.Title;
        Performers = string.Join(", ", Tags.Performers);
        FullTitle = $"{Performers} - {Title}";
    }

    public string? LocalPath { get; }
    public byte[] AudioData { get; }
    public string FileName { get; }
    public Tag Tags { get; }

    public string FullTitle { get; }
    public string Title { get; }
    public string Performers { get; }
}