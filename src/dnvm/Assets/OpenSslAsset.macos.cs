using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.VersionManager.Assets
{
    /// <summary>
    /// Represents the linking of openssl into installations of Microsoft.NETCore.App on macOS.
    /// </summary>
    public class OpenSslAsset : Asset
    {
        private readonly string _symlinkDest;
        private readonly string[] _dylibs = new[] { "libcrypto.1.0.0.dylib", "libssl.1.0.0.dylib" };

        public override string DisplayName { get; } = "OpenSSL (from Homebrew)";

        /// <summary>
        /// </summary>
        /// <param name="symlinkDest">The folder in which to symlink openssl</param>
        public OpenSslAsset(string symlinkDest)
        {
            _symlinkDest = symlinkDest;
        }

        public override Task<bool> InstallAsync(CancellationToken cancellationToken)
        {
            var basename = GetOpenSSLPath();
            foreach (var dylib in _dylibs)
            {
                var src = Path.Combine(basename, dylib);
                if (!File.Exists(src))
                {
                    return Task.FromResult(false);
                }

                var dest = Path.Combine(_symlinkDest, dylib);
                if (!cancellationToken.IsCancellationRequested)
                {
                    symlink(src, dest);
                }
            }

            return Task.FromResult(true);
        }

        [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
        private extern static void symlink(string src, string dest);

        public override bool Uninstall()
        {
            foreach (var dylib in _dylibs)
            {
                var filePath = Path.Combine(_symlinkDest, dylib);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            return true;
        }

        private string GetOpenSSLPath()
        {
            // default Homebrew library path
            return "/usr/local/opt/openssl/lib/";
        }
    }
}
