using System.IO;
using System.Linq;

namespace DotNet.VersionManager.Files
{
    public class ConfigFileYamlWriter
    {
        // TODO currently overwrites the file...which removes comments
        // TODO consider using real yaml serializer. Couldn't figure out how to make YamlDotNet suppress trailing '...' at EOD
        public void Write(TextWriter writer, ConfigFile config)
        {
            if (!string.IsNullOrEmpty(config.Environment))
            {
                writer.Write($"env: {config.Environment}\n");
            }

            if (!string.IsNullOrEmpty(config.Sdk))
            {
                writer.Write($"sdk: {config.Sdk}\n");
            }

            if (config.Runtime.Count > 0)
            {
                writer.Write("runtime:");
                if (config.Runtime.Count == 1)
                {
                    writer.Write($" {config.Runtime.First()}\n");
                }
                else
                {
                    writer.Write("\n");
                    foreach (var runtime in config.Runtime)
                    {
                        writer.Write($"  - {runtime}\n");
                    }
                }
            }

            if (config.Tools.Count > 0)
            {
                writer.Write("tools:\n");
                foreach (var tool in config.Tools)
                {
                    writer.Write($"  {tool.Key}: {tool.Value}\n");
                }
            }
        }
    }
}
