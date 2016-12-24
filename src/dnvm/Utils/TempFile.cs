using System;
using System.IO;

namespace DotNet.Utils
{
    public class TempFile : IDisposable
    {
        public TempFile()
        {
            Path = System.IO.Path.GetTempFileName();
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