using System;
using TagLib;
using System.IO;
using System.Security.Cryptography;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

using File = System.IO.File;
using TagFile = TagLib.File;

namespace Eclair;

public class Audio : Core.IAudio
{
    void InitializeMetadata()
    {
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
    
#pragma warning disable CS8618
    public Audio(IStorageFile file, Stream stream)
    {
        LocalPath = file.TryGetLocalPath();
    
        var reader = new BinaryReader(stream);
        AudioData = reader.ReadBytes((int)stream.Length);
    
        FileName = file.Name;
        Tags = TagFile.Create(new StorageFileAbstraction(file.Name, stream)).Tag;
    
        InitializeMetadata();
    }
    
    public Audio(string path)
    {
        LocalPath = path;
        AudioData = File.ReadAllBytes(path);
        FileName = Path.GetFileName(path);
        Tags = TagFile.Create(path).Tag;
    
        InitializeMetadata();
    }
#pragma warning restore CS8618

    public string GetHash() => Convert.ToHexStringLower( SHA256.HashData(AudioData));

    public string? LocalPath { get; }
    public byte[] AudioData { get; }
    public string FileName { get; }
    public Tag Tags { get; }

    public Image? Image { get; private set; }
    public string FullTitle { get; private set; }
    public string Title { get; private set; }
    public string Performers { get; private set; }
    
    public static bool operator ==(Audio? left, Audio? right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;
        return left.GetHash() == right.GetHash();
    }
    
    public static bool operator !=(Audio? left, Audio? right) => !(left == right);
    
    public override bool Equals(object? obj) => obj is Audio audio && this == audio;
    
    public override int GetHashCode() => GetHash().GetHashCode();
}