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

public class Audio : Core.ISong, IDisposable
{
    void InitializeMetadata()
    {
        if (Tags.Pictures.Length > 0)
        {
            var image = Tags.Pictures[0];
            using MemoryStream ms = new(image.Data.Data);
            try
            {
                Image = new() { Source = new Bitmap(ms) };
            }
            catch (Exception ex)
            {
                Logger.Instance.Warn($"Caught {ex.GetType()}: '{ex.Message}'.");
            }
        }
    
        Title = Tags.Title;
        if (string.IsNullOrEmpty(Title))
            Title = FileName;
        Performers = string.Join(", ", Tags.Performers);
        Album = Tags.Album;
    }
    
#pragma warning disable CS8618
    public Audio(IStorageFile file, Stream stream)
    {
        LocalPath = file.TryGetLocalPath();

        _stream = stream;
    
        FileName = file.Name;
        Tags = TagFile.Create(new StorageFileAbstraction(file.Name, stream)).Tag;
    
        InitializeMetadata();
    }
    
    public Audio(string path)
    {
        LocalPath = path;
        FileName = Path.GetFileName(path);
        Tags = TagFile.Create(path).Tag;
    
        InitializeMetadata();
    }
#pragma warning restore CS8618

    readonly Stream? _stream;
    byte[]? _cachedData;
    public string? LocalPath { get; }

    public byte[] AudioData
    {
        get
        {
            if (_cachedData != null)
                return _cachedData;
            
            if (_stream != null)
            {
                _stream.Position = 0;
                using var ms = new MemoryStream();
                _stream.CopyTo(ms);
                _cachedData = ms.ToArray();
            }
            else
            {
                _cachedData = File.ReadAllBytes(LocalPath!);
            }
            
            return _cachedData;
        }
    }
    public string FileName { get; }
    public Tag Tags { get; }

    public Image? Image { get; private set; }
    public string Title { get; private set; }
    public string Performers { get; private set; }
    public string Album { get; private set; }

    public void Dispose()
    {
        _stream?.Dispose();
        GC.SuppressFinalize(this);
    }
}