using System;
using System.IO;

namespace DotNet.Utils
{
    public class ArchiveUtility
    {
        public static bool Extract(string filename, string destination)
        {
            Directory.CreateDirectory(destination);

            if (filename.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase)
                || filename.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
            {
                return TarballExtractor.Extract(filename, destination, gzipped: true);
            }
            else if (filename.EndsWith(".nupkg", StringComparison.OrdinalIgnoreCase)
                || filename.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                return ZipExtractor.Extract(filename, destination);
            }
            else
            {
                throw new InvalidOperationException("Unrecognized file extension");
            }
        }
    }
}