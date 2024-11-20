namespace EclairLib.Audio;

public abstract class FileReader<T> where T : IAudio
{
    public virtual T Read(Stream stream, bool leaveOpen = false) => Read(new BinaryReader(stream), leaveOpen);
    public abstract T Read(BinaryReader reader, bool leaveOpen = false);
    public virtual T Read(string fpath) => Read(new BinaryReader(File.OpenRead(fpath)));
}