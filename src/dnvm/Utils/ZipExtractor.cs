using System.IO;
using System.IO.Compression;

namespace DotNet.VersionManager.Utils
{
    public class ZipExtractor
    {
        public static bool Extract(string file, string destination)
        {
            using (var filestream = new FileStream(file, FileMode.Open))
            using (var zip = new ZipArchive(filestream))
            {
                foreach (var entry in zip.Entries)
                {
                    var dest = Path.Combine(destination, entry.FullName);
                    Directory.CreateDirectory(Path.GetDirectoryName(dest));
                    entry.ExtractToFile(dest);
                }
            }

            return true;
        }
    }
}
