using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DotNet.VersionManager.Assets
{
    /// <summary>
    /// Represents the linking of openssl into installations of Microsoft.NETCore.App on macOS.
    /// </summary>
    public class OpenSslAsset : Asset
    {
        private readonly ILogger _logger;
        private readonly string _symlinkDest;
        private readonly string[] _dylibs;
        private readonly string _netCoreVersion;

        public override string DisplayName { get; }

        public override IEnumerable<Asset> Dependencies
            => Enumerable.Empty<Asset>();

        public override bool IsInstalled
            => _dylibs.All(File.Exists);

        /// <summary>
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="symlinkDest">The folder in which to symlink openssl</param> <summary>
        /// <param name="netCoreVersion">The version of .NET Core (for display purposes only)</param>
        public OpenSslAsset(ILogger logger, string symlinkDest, string netCoreVersion)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _symlinkDest = Ensure.NotNullOrEmpty(symlinkDest, nameof(symlinkDest));
            _netCoreVersion = Ensure.NotNullOrEmpty(netCoreVersion, nameof(netCoreVersion));
            _dylibs = new[]
            {
                Path.Combine(symlinkDest, "libcrypto.1.0.0.dylib"),
                Path.Combine(symlinkDest, "libssl.1.0.0.dylib"),
            };

            DisplayName = $"OpenSSL for .NET Core {_netCoreVersion}";
        }

        public override Task<bool> InstallAsync(CancellationToken cancellationToken)
        {
            Debug.Assert(!IsInstalled);

            _logger.Output($"Linking OpenSSL into .NET Core {_netCoreVersion}");

            var basename = GetOpenSSLPath();
            foreach (var dylib in _dylibs)
            {
                var src = Path.Combine(basename, Path.GetFileName(dylib));
                if (!File.Exists(src))
                {
                    _logger.Warn($"Failed to install OpenSSL into {DisplayName}. Try running `brew install openssl` first and re-run this command.");
                    return Task.FromResult(false);
                }

                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.Verbose($"Linking '{src}' to '{dylib}'");
                    symlink(src, dylib);
                }
            }

            return Task.FromResult(true);
        }

        [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
        private extern static void symlink(string src, string dest);

        public override bool Uninstall()
        {
            Debug.Assert(IsInstalled);

            foreach (var dylib in _dylibs)
            {
                if (File.Exists(dylib))
                {
                    File.Delete(dylib);
                }
            }

            return true;
        }

        private static string GetOpenSSLPath()
        {
            // default Homebrew library path
            return "/usr/local/opt/openssl/lib/";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var other = (OpenSslAsset)obj;
            return this._symlinkDest.Equals(other._symlinkDest);
        }

        public override int GetHashCode()
        {
            return _symlinkDest.GetHashCode();
        }
    }
}
