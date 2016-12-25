using System.IO;

namespace DotNet.Files
{
    public class ConfigFileFactory
    {
        public string FindFile(string directory)
        {
            var current = new DirectoryInfo(directory);
            FileInfo file = null;
            while (current != null)
            {
                var candidate = new FileInfo(Path.Combine(current.FullName, FileConstants.Config));
                if (candidate.Exists)
                {
                    file = candidate;
                    break;
                }
                current = current.Parent;
            }
            return file.FullName;
        }

        public ConfigFile Create(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            using (var reader = new StreamReader(fileStream))
            {
                var parsed = new ConfigFileYamlReader().Read(reader);
                parsed.FilePath = filePath;
                return parsed;
            }
        }
    }
}