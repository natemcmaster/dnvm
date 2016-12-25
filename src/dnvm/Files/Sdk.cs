using System.IO;

namespace DotNet.Files
{
    public class Cli
    {
        private readonly DirectoryInfo _base;
        internal Cli(DirectoryInfo @base)
        {
            _base = @base;
        }

        public string Version => _base.Name;
    }
}