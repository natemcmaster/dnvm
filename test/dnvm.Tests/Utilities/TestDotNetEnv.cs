using System;
using System.IO;
using DotNet.VersionManager.Files;
using DotNet.VersionManager.Utils;

namespace DotNet.VersionManager.Tests
{
    public class TestDotNetEnv : DotNetEnv, IDisposable
    {
        private TestDotNetEnv(string name, DirectoryInfo root)
            : base(name, root)
        {
        }

        public static DotNetEnv Create()
        {
            var tempDir = new TempDir();
            return new TestDotNetEnv("test", new DirectoryInfo(tempDir.Path));
        }

        public void Dispose()
        {
            Directory.Delete(Root, recursive: true);
        }
    }
}
