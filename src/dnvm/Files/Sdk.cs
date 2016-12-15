using System.IO;

namespace DotNet.Files
{
    public class Sdk
    {
        private readonly DirectoryInfo _base;
        internal Sdk(DirectoryInfo @base)
        {
            _base = @base;
        }

        public string Version => _base.Name;
    }
}