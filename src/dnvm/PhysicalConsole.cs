using System;
using System.IO;

namespace DotNet
{
    class PhysicalConsole : IConsole
    {
        private PhysicalConsole()
        {}

        public static readonly IConsole Instance = new PhysicalConsole();

        public TextWriter Out => Console.Out;
        public TextWriter Error => Console.Error;

        public IConsole WriteLine(string line)
        {
            Console.WriteLine(line);
            return this;
        }
    }
}