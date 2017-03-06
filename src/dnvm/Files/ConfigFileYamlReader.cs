using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace DotNet.VersionManager.Files
{
    public static class ConfigFileErrors
    {
        public const string EnvIsNotScalar = "The value for 'env' must be a single, scalar value.";
        public const string SdkIsNotScalar = "The value for 'sdk' must be a single, scalar value.";
        public const string RuntimeSequenceItemIsNotScalar = "Items in the 'runtime' section must only be scalar values.";
        public const string RuntimeMustBeListOrScalar = "The 'runtime' section must be a single value or a sequence of scalar values.";
        public const string MissingEnvKey = "Missing the required 'env' key.";
        public const string MultipleDocuments = "The config file should not contain multiple YAML document sections.";
        public const string ToolsMustBeMap = "The 'tools' section must only contain key/value pairs for 'tool_name': 'tool_version'";
        public const string ToolItemsMustBeScalar = "Items in the 'tools' section must only contain key/value pairs for 'tool_name': 'tool_version'";
    }


    public class ConfigFileYamlReader
    {
        public ConfigFile Read(TextReader reader)
        {
            try
            {
                var configFile = new ConfigFile();
                var stream = new YamlStream();
                stream.Load(reader);

                if (stream.Documents.Count == 1)
                {
                    ReadDocument(stream.Documents[0], configFile);
                }
                else if (stream.Documents.Count > 1)
                {
                    throw new FormatException(ConfigFileErrors.MultipleDocuments);
                }

                Validate(configFile);

                return configFile;
            }
            catch (YamlException ex)
            {
                throw new FormatException(ex.Message, ex);
            }
        }

        private void Validate(ConfigFile file)
        {
            if (string.IsNullOrEmpty(file.Environment))
            {
                throw new FormatException(ConfigFileErrors.MissingEnvKey);
            }
        }

        private static void ReadDocument(YamlDocument doc, ConfigFile configFile)
        {
            if (doc.RootNode is YamlMappingNode rootMap)
            {
                foreach (var item in rootMap.Children)
                {
                    if (item.Key is YamlScalarNode key)
                    {
                        ReadKey(key, item.Value, configFile);
                    }
                }
            }
        }

        private static void ReadKey(YamlScalarNode key, YamlNode node, ConfigFile configFile)
        {
            switch (key.Value.ToLowerInvariant())
            {
                case "env":
                    {
                        if (node is YamlScalarNode env)
                        {
                            configFile.Environment = env.Value;
                        }
                        else
                        {
                            throw new FormatException(ConfigFileErrors.EnvIsNotScalar);
                        }
                    }
                    break;
                case "runtime":
                    {
                        ReadFx(node, configFile);
                    }
                    break;
                case "sdk":
                    {
                        if (node is YamlScalarNode sdk)
                        {
                            configFile.Sdk = sdk.Value;
                        }
                        else
                        {
                            throw new FormatException(ConfigFileErrors.SdkIsNotScalar);
                        }
                    }
                    break;
                case "tools":
                    {
                        if (node is YamlMappingNode tools)
                        {
                            foreach (var tool in tools)
                            {
                                var name = tool.Key as YamlScalarNode;
                                if (!(tool.Value is YamlScalarNode version))
                                {
                                    throw new FormatException(ConfigFileErrors.ToolItemsMustBeScalar);
                                }

                                configFile.Tools[name.Value] = version.Value;
                            }
                        }
                        else
                        {
                            throw new FormatException(ConfigFileErrors.ToolsMustBeMap);
                        }
                    }
                    break;
            }
        }

        private static void ReadFx(YamlNode node, ConfigFile configFile)
        {
            if (node is YamlScalarNode runtime)
            {
                configFile.Runtime.Add(runtime.Value);
            }
            else if (node is YamlSequenceNode values)
            {
                foreach (var runtimeItem in values.Children)
                {
                    if (runtimeItem is YamlScalarNode runtimeValue)
                    {
                        configFile.Runtime.Add(runtimeValue.Value);
                    }
                    else
                    {
                        throw new FormatException(ConfigFileErrors.RuntimeSequenceItemIsNotScalar);
                    }
                }
            }
            else
            {
                throw new FormatException(ConfigFileErrors.RuntimeMustBeListOrScalar);
            }
        }
    }
}
