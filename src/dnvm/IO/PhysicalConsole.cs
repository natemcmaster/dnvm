using System;
using System.IO;

namespace DotNet.VersionManager.IO
{
    class PhysicalConsole : IConsole
    {
        private object _write = new object();

        private PhysicalConsole()
        { }

        public static readonly IConsole Instance = new PhysicalConsole();

        public TextWriter Out => Console.Out;
        public TextWriter Error => Console.Error;
        public TextReader In => Console.In;

        public IConsole WriteLine(string line, ConsoleColor? foreground = null)
        {
            lock (_write)
            {
                if (foreground.HasValue)
                {
                    Console.ForegroundColor = foreground.Value;
                }

                Console.WriteLine(line);

                if (foreground.HasValue)
                {
                    Console.ResetColor();
                }
            }
            return this;
        }

        public IConsole Write(string text, ConsoleColor? foreground = null)
        {
            lock (_write)
            {
                if (foreground.HasValue)
                {
                    Console.ForegroundColor = foreground.Value;
                }

                Console.Write(text);

                if (foreground.HasValue)
                {
                    Console.ResetColor();
                }
            }
            return this;
        }
    }
}
