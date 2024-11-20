using System.IO;

namespace Eclair;

internal class ReadOnlyFileImplementation(string name, Stream readstream) : TagLib.File.IFileAbstraction
{
    public string Name { get; } = name;
    public Stream ReadStream { get; } = readstream;
    public Stream WriteStream => throw new System.NotImplementedException();

    // RUS: ���� ������� Stream, �� ����� ������� ��������� �����, ��� ��� Stream �� ������ ����������� ��-�� ������ LibVLC#.
    // ENG: if you close the Stream via this method, then you will have to open a new one, since the Stream should not be closed due to the operation of LibVLC#.
    public void CloseStream(Stream stream) { }
}