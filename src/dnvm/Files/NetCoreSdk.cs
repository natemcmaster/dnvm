using System.IO;

namespace DotNet.Files
{
    public class NetCoreSdk
    {
        private readonly DirectoryInfo _base;
        internal NetCoreSdk(DirectoryInfo @base)
        {
            _base = @base;
        }

        public string Version => _base.Name;
    }
}