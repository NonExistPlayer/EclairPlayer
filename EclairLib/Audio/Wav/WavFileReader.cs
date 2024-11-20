namespace EclairLib.Audio.Wav;

public class WavFileReader : FileReader<WavAudio>
{
    public override WavAudio Read(BinaryReader reader, bool leaveOpen = true)
    {
        var idex = new InvalidDataException("Not a valid WAV file.");
        if (new string(reader.ReadChars(4)) != "RIFF")
            throw idex;

        reader.ReadInt32(); // file size

        if (new string(reader.ReadChars(4)) != "WAVE")
            throw idex;

        if (new string(reader.ReadChars(4)) != "fmt ")
            throw idex;

        reader.ReadInt32(); // Размер формата
        reader.ReadInt16(); // Формат (например, PCM)
        short chnls = reader.ReadInt16();
        int sRate = reader.ReadInt32();
        reader.ReadInt16(); // Битрейт
        reader.ReadInt16(); // Бит на сэмпл

        if (new string(reader.ReadChars(4)) != "data")
            throw idex;

        int dataSize = reader.ReadInt32();
        byte[] aData = reader.ReadBytes(dataSize);

        if (leaveOpen)
            reader.Close();

        return new(sRate, chnls, aData);
    }
}