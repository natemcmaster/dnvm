using System.Collections.Generic;

namespace DotNet.VersionManager.Files
{
    public class ConfigFile
    {
        public string FilePath { get; set; }

        public string Environment { get; set; }

        public string Sdk { get; set; }

        public IDictionary<string, string> Tools { get; } = new Dictionary<string, string>();

        public ISet<string> SharedFx { get; } = new HashSet<string>();
    }
}
