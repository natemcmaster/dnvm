using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Assets
{
    /// <summary>
    /// Represents the linking of openssl into installations of Microsoft.NETCore.App on macOS.
    /// </summary>
    public class OpenSslAsset : Asset
    {
        private readonly string _symlinkDest;

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
            // default Homebrew library path
            var basename = "/usr/local/opt/openssl/lib/";
            foreach (var file in new[] { "libcrypto.1.0.0.dylib", "libssl.1.0.0.dylib" })
            {
                var src = Path.Combine(basename, file);
                if (!File.Exists(src))
                {
                    return Task.FromResult(false);
                }

                var dest = Path.Combine(_symlinkDest, file);
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
            throw new NotImplementedException();
        }
    }
}