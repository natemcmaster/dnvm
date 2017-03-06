using DotNet.VersionManager.IO;

namespace DotNet.VersionManager
{
    public static class ConsoleExtensions
    {
        public static string Prompt(this IConsole console, string question)
        {
            console.Write(question);
            return console.In.ReadLine();
        }
    }
}
