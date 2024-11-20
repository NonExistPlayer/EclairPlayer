namespace EclairLib.Audio;

public interface IAudio
{
    public int SampleRate { get; }
    public short Channels { get; }
    public byte[] AudioData { get; }

    public double Duration { get; }
}