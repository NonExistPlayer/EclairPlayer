using TagLib;
using System.IO;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
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

        if (Tags.Pictures.Length > 0)
        {
            var image = Tags.Pictures[0];
            using MemoryStream ms = new(image.Data.Data);
            Image = new() { Source = new Bitmap(ms) };
        }

        Title = Tags.Title;
        if (string.IsNullOrEmpty(Title))
            Title = FileName;
        Performers = string.Join(", ", Tags.Performers);
        FullTitle = !string.IsNullOrWhiteSpace(Performers) ?
                    $"{Performers} - " :
                    "";
        FullTitle += Title;
    }

    public Audio(string path)
    {
        LocalPath = path;
        AudioData = File.ReadAllBytes(path);
        FileName = Path.GetFileName(path);
        Tags = TagFile.Create(path).Tag;

        if (Tags.Pictures.Length > 0)
        {
            var image = Tags.Pictures[0];
            using MemoryStream ms = new(image.Data.Data);
            Image = new() { Source = new Bitmap(ms) };
        }

        Title = Tags.Title;
        Performers = string.Join(", ", Tags.Performers);
        FullTitle = $"{Performers} - {Title}";
    }

    public string? LocalPath { get; }
    public byte[] AudioData { get; }
    public string FileName { get; }
    public Tag Tags { get; }

    public Image? Image { get; }
    public string FullTitle { get; }
    public string Title { get; }
    public string Performers { get; }
}