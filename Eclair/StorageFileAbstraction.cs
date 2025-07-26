using System;
using System.IO;

namespace Eclair;

public class StorageFileAbstraction(string name, Stream read) : TagLib.File.IFileAbstraction
{
    public string Name { get; } = name;
    public Stream ReadStream { get; } = read;
    public Stream WriteStream => throw new NotImplementedException();

    public void CloseStream(Stream stream) => stream.Close();
}