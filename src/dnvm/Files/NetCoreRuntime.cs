using System.IO;

namespace DotNet.VersionManager.Files
{
    public class NetCoreRuntime
    {
        public NetCoreRuntime(DirectoryInfo @base)
        {
            Version = @base.Name;
            Name = @base.Parent.Name;
        }

        public string Name { get; }
        public string Version { get; }
    }
}
