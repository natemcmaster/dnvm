using System.IO;

namespace DotNet.Files
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

            if (config.SharedFx.Count > 0)
            {
                writer.Write("fx:");
                if (config.SharedFx.Count == 1)
                {
                    writer.Write($" {config.SharedFx[0]}\n");
                }
                else
                {
                    writer.Write("\n");
                    foreach (var fx in config.SharedFx)
                    {
                        writer.Write($"  - {fx}\n");
                    }
                }
            }
        }
    }
}