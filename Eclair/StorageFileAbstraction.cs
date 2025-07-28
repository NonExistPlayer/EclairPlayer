using System;
using System.IO;

namespace Eclair;

public class StorageFileAbstraction(string name, Stream read) : TagLib.File.IFileAbstraction
{
    public string Name { get; } = name;
    public Stream ReadStream { get; } = read;
    public Stream WriteStream => throw new NotImplementedException();

    // гарантирует что не будет закрыт Stream, который используются в Audio. 
    public void CloseStream(Stream stream) {}
}