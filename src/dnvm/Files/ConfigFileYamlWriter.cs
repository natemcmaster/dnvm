using System.IO;
using YamlDotNet.RepresentationModel;

namespace DotNet.Files
{
    public class ConfigFileYamlWriter
    {
        // TODO currently overwrites the file...which removes comments
        // TODO suppress the trailing '...' added to each file
        public void Write(TextWriter writer, ConfigFile config)
        {
            var stream = new YamlStream();

            var root = new YamlMappingNode();

            if (!string.IsNullOrEmpty(config.Environment))
            {
                root.Add("env", config.Environment);
            }

            if (!string.IsNullOrEmpty(config.Sdk))
            {
                root.Add("sdk", config.Sdk);
            }

            if (config.SharedFx.Count > 0)
            {
                var frameworks = new YamlSequenceNode();
                foreach (var fx in config.SharedFx)
                {
                    frameworks.Add(fx);
                }
                root.Add("fx", frameworks);
            }

            stream.Add(new YamlDocument(root));
            stream.Save(writer, assignAnchors: false);
        }
    }
}