using System;

namespace DotNet.Utils
{
    public class ArchiveUtility
    {
        public static bool Extract(string filename, string destination)
        {
            if (filename.EndsWith(".tar.gz") || filename.EndsWith(".tgz"))
            {
                return TarballExtractor.Extract(filename, destination, gzipped: true);
            }
            else
            {
                throw new InvalidOperationException("Unrecognized file extension");
            }
        }
    }
}