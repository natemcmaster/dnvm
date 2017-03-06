using System;
using System.IO;

namespace DotNet.VersionManager
{
    public interface IConsole
    {
        IConsole Write(string text, ConsoleColor? foreground = null);
        IConsole WriteLine(string line, ConsoleColor? foreground = null);
        TextWriter Out { get; }
        TextWriter Error { get; }
        TextReader In { get; }
    }
}
