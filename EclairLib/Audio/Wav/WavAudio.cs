namespace EclairLib.Audio.Wav;

public class WavAudio : IAudio
{
    internal WavAudio(int sRate, short chnls, byte[] aData)
    {
        SampleRate = sRate;
        Channels = chnls;
        AudioData = aData;
    }

    public int SampleRate { get; }
    public short Channels { get; }
    public byte[] AudioData { get; }

    public double Duration { get; }
}