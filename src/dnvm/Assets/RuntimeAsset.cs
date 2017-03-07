using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DotNet.VersionManager.Files;
using Microsoft.Extensions.Logging;

namespace DotNet.VersionManager.Assets
{
    public class RuntimeAsset : DotNetAssetBase
    {
        public const string DefaultVersion = "stable";
        private const string RuntimeName = "Microsoft.NETCore.App";

        private readonly AssetInfo _assetInfo;
        private readonly DotNetEnv _env;
        private readonly Architecture _arch;
        private static readonly HttpClient DefaultHttpClient = new HttpClient();

        public RuntimeAsset(ILogger logger, DotNetEnv env, string version, Architecture arch)
            : base(logger)
        {
            _arch = arch;
            var assetId = CreateAssetId(arch);
            _assetInfo = version == DefaultVersion
                ? Channel.GetLatest(assetId)
                : Channel.GetAssetInfo(assetId, version);

            _env = env;
            DisplayName = $".NET Core Runtime {Version}";
#if FEATURE_MULTI_ARCH_ASSETS
            DisplayName += " ({arch.ToString().ToLower()})";
#endif
        }

        public string Version => _assetInfo.Version;

        public static string CreateAssetId(Architecture arch)
            => $"{RuntimeName}.{PlatformConstants.RuntimeOSName}-{arch.ToString().ToLower()}";

        public override string DisplayName { get; }

        public override IEnumerable<Asset> Dependencies
#if __MACOS__
            => new[] { new OpenSslAsset(Log, GetInstallationPath(), _assetInfo.Version) };
#else
            => Enumerable.Empty<Asset>();
#endif

        public override bool IsInstalled
            => _env.Runtimes.Any(f => f.Name.Equals(RuntimeName, StringComparison.OrdinalIgnoreCase) && f.Version == Version);

        public override bool Uninstall()
        {
            Log.Output($"Deleting files for {DisplayName}");
            return UninstallFolder(GetInstallationPath());
        }

        public override async Task<bool> InstallAsync(CancellationToken cancellationToken)
        {
            Log.Verbose($"Begin installation of {DisplayName} to '{_env.Root}'");

            var dest = GetInstallationPath();

            Directory.CreateDirectory(dest);

            var url = _assetInfo.DownloadUrl;

            Log.Output($"Downloading {DisplayName}");
            if (!await DownloadAndExtractAsync(url, _env.Root, cancellationToken))
            {
                Log.Error($"Failed to install {DisplayName}");

                if (Directory.EnumerateFiles(dest).Any())
                {
                    try
                    {
                        Directory.Delete(dest, recursive: true);
                    }
                    catch
                    {
                        Log.Verbose($"Failed to delete {dest}");
                    }
                }

                return false;
            }

            Log.Output($"Installed {DisplayName}");

            return true;
        }

        private string GetInstallationPath()
            => Path.Combine(_env.FxRoot, RuntimeName, Version);

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

            var other = (RuntimeAsset)obj;
            return _assetInfo.Version == other._assetInfo.Version
                && _arch == other._arch
                && _env.Name == other._env.Name;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + _arch.GetHashCode();
                hash = hash * 23 + _assetInfo.Version.GetHashCode();
                hash = hash * 23 + _env.Name.GetHashCode();
                return hash;
            }
        }
    }
}
