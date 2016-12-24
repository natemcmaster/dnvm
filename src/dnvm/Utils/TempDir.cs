using System;
using System.IO;

namespace DotNet.Utils
{
    using IOPath = System.IO.Path;

    public class TempDir : IDisposable
    {
        public TempDir()
        {
            Path = IOPath.Combine(IOPath.GetTempPath(), Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
    }
}