using System.IO;

namespace DotNet
{
    public interface IConsole
    {
        IConsole Write(string text);
        IConsole WriteLine(string line);
        TextWriter Out { get; }
        TextWriter Error { get; }
        TextReader In { get; }
    }
}