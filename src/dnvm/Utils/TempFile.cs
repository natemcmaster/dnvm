using System;
using System.IO;

namespace DotNet.VersionManager.Utils
{
    public class TempFile : IDisposable
    {
        public TempFile()
            :this(extension: null)
        {
        }

        public TempFile(string extension)
        {
            Path = System.IO.Path.GetTempFileName() + extension;
        }

        public string Path { get; }

        public void Dispose()
        {
            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
        }
    }
}
