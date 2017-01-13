
using System.IO;

namespace DotNet.Files
{
    public class NetCoreFramework : Framework
    {
        public NetCoreFramework() { }

        internal NetCoreFramework(DirectoryInfo @base)
        {
            Version = @base.Name;
            Name = @base.Parent.Name;
        }
    }
}