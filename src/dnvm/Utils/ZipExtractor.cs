using System.IO;
using System.IO.Compression;

namespace DotNet.VersionManager.Utils
{
    public class ZipExtractor
    {
        public static bool Extract(string path, string destination)
        {
            using (var file = File.OpenRead(path))
            using (var zip = new ZipArchive(file))
            {
                zip.ExtractToDirectory(destination);
            }

            return true;
        }
    }
}
