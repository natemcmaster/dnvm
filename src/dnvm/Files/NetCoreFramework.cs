
using System.IO;

namespace DotNet.VersionManager.Files
{
    public class NetCoreFramework : Framework
    {
        internal NetCoreFramework(DirectoryInfo @base)
        {
            Version = @base.Name;
            Name = @base.Parent.Name;
        }
    }
}
