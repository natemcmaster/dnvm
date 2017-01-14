using System.IO;

namespace DotNet.Files
{
    public class NetCoreTool
    {
        public NetCoreTool(DirectoryInfo @base)
        {
            Name = @base.Parent.Name;
            Version = @base.Name;
        }

        public string Name { get; }
        public string Version { get; }
    }
}