using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DotNet
{
    public class DnvmSettings
    {
        private DnvmSettings()
        {
        }

        public static DnvmSettings Load()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new InvalidOperationException("Currently only macOS is supported");
            }

            var installRoot = new DirectoryInfo("/usr/local/share/dnvm");
            return new DnvmSettings
            {
                InstallRoot = installRoot,
                EnvRoot = new DirectoryInfo(Path.Combine(installRoot.FullName, "environments")),
            };
        }

        public DirectoryInfo InstallRoot { get; private set; }
        public DirectoryInfo EnvRoot { get; private set; }
    }
}