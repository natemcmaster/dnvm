using System.Collections.Generic;

namespace DotNet.Files
{
    public class ConfigFile
    {
        public string FilePath { get; set; }

        public string Environment { get; set; }

        public string Sdk { get; set; }

        public IList<string> SharedFx { get; } = new List<string>();
    }
}