using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DotNet.Files
{
    public class ToolManifest
    {
        public static ToolManifest LoadFromFile(string fileName)
        {
            try
            {
                return JsonConvert.DeserializeObject<ToolManifest>(File.ReadAllText(fileName));
            }
            catch (JsonException ex)
            {
                throw new FormatException("Invalid tool manifest format", ex);
            }
        }

        public int Version { get; set; }
        public IDictionary<string, Command> Commands { get; set; } = new Dictionary<string, Command>();

        public class Command
        {
            public string Exe { get; set; }
            public bool Portable { get; set; }
            public string RuntimeConfig { get; set; }
        }
    }
}