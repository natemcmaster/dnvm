using System.IO;

namespace DotNet
{
    public interface IConsole
    {
        IConsole WriteLine(string line);
        TextWriter Out { get; }
        TextWriter Error { get; }
    }
}