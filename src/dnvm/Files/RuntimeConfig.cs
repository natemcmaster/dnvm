using System;
using System.IO;
using Newtonsoft.Json;

namespace DotNet.VersionManager.Files
{
    public class RuntimeConfig
    {
        public static RuntimeConfig LoadFromFile(string fileName)
        {
            try
            {
                return JsonConvert.DeserializeObject<RuntimeConfig>(File.ReadAllText(fileName));
            }
            catch (JsonException ex)
            {
                throw new FormatException("Invalid runtime config format", ex);
            }
        }

        public RuntimeConfigOptions RuntimeOptions { get; set; }

        public class RuntimeConfigOptions
        {
            public RuntimeFramework Framework { get; set; }
        }

        public class RuntimeFramework
        {
            public string Name { get; set; }
            public string Version { get; set; }
        }
    }
}
