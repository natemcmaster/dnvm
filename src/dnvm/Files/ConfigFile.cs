using System.Collections.Generic;

namespace DotNet.VersionManager.Files
{
    public class ConfigFile
    {
        public string FilePath { get; set; }

        public string EnvName { get; set; }

        public string Sdk { get; set; }

        public IDictionary<string, string> Tools { get; } = new Dictionary<string, string>();

        public ISet<string> Runtimes { get; } = new HashSet<string>();
    }
}
