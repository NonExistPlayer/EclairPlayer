using File = System.IO.File;
using System.IO;
using TagLib;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace Eclair;

public class Media
{
    private Bitmap? _bitmap;
    
    public Media(string path)
    {
        FileName = Path.GetFileName(path);
        
        Tags = TagLib.File.Create(path).Tag;

        AudioData = File.ReadAllBytes(path);

        LocalPath = path;
    }

    public Media(IStorageFile file)
    {
        FileName = file.Name;

        LocalPath = file.TryGetLocalPath();

        var task = file.OpenReadAsync();

        task.Wait();

        using Stream stream = task.Result;

        Tags = TagLib.File.Create(new ReadOnlyFileImplementation(file.Name, stream)).Tag;

        stream.Position = 0;

        BinaryReader reader = new(stream);

        AudioData = reader.ReadBytes((int)stream.Length);
    }

    public Media(string name, Stream stream, bool closestream = true)
    {
        FileName = name;

        Tags = TagLib.File.Create(new ReadOnlyFileImplementation(name, stream)).Tag;

        stream.Position = 0;

        BinaryReader reader = new(stream);

        AudioData = reader.ReadBytes((int)stream.Length);

        if (closestream)
            stream.Close();
    }

    public string? LocalPath { get; }

    public byte[] AudioData { get; }

    public string FileName { get; }

    public Bitmap? Image
    {
        get
        {
            if (_bitmap == null)
            {
                IPicture? picture = Tags.Pictures.Length > 0 ? Tags.Pictures[0] : null;
                if (picture != null)
                {
                    string fpath = TempPath + $"{Title}-picture0";

                    if (File.Exists(fpath))
                        goto display;

                    var outputstream = File.OpenWrite(fpath);
                    outputstream.Write(picture.Data.Data, 0, picture.Data.Count);
                    outputstream.Close();

                display:
                    _bitmap = new Bitmap(fpath);
                }
            }
            
            return _bitmap;
        }
    }
    public string FullTitle
    {
        get
        {
            if (Title == "" || Performers == "")
                return FileName;
            return Performers + " - " + Title; 
        }
    }

    public string Title => Tags.Title;
    public string Performers => string.Join(", ", Tags.Performers);

    public Tag Tags { get; }

}