using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace DotNet.VersionManager.Files
{
    public static class ConfigFileErrors
    {
        public const string EnvNameIsNotScalar = "The value for 'envName' must be a single, scalar value.";
        public const string SdkIsNotScalar = "The value for 'sdk' must be a single, scalar value.";
        public const string RuntimeSequenceItemIsNotScalar = "Items in the 'runtimes' section must only be scalar values.";
        public const string RuntimeMustBeListOrScalar = "The 'runtimes' section must be a single value or a sequence of scalar values.";
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

                return configFile;
            }
            catch (YamlException ex)
            {
                throw new FormatException(ex.Message, ex);
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
            switch (key.Value)
            {
                case "envName":
                    {
                        if (node is YamlScalarNode env)
                        {
                            configFile.EnvName = env.Value;
                        }
                        else
                        {
                            throw new FormatException(ConfigFileErrors.EnvNameIsNotScalar);
                        }
                    }
                    break;
                case "runtimes":
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
                configFile.Runtimes.Add(runtime.Value);
            }
            else if (node is YamlSequenceNode values)
            {
                foreach (var runtimeItem in values.Children)
                {
                    if (runtimeItem is YamlScalarNode runtimeValue)
                    {
                        configFile.Runtimes.Add(runtimeValue.Value);
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
