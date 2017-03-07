using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using DotNet.VersionManager.Files;
using Microsoft.Extensions.Logging;

namespace DotNet.VersionManager.Assets
{
    public class SdkAsset : DotNetAssetBase
    {
        public const string DefaultVersion = "stable";
        private const string AssetIdPrexix = "Microsoft.DotNet.Cli";
        private readonly DotNetEnv _env;
        private readonly AssetInfo _assetInfo;
        private readonly Architecture _arch;

        public SdkAsset(ILogger logger, DotNetEnv env, string version, Architecture arch)
            : base(logger)
        {
            _arch = arch;
            var assetId = CreateAssetId(arch);

            _assetInfo = (version == DefaultVersion)
                ? Channel.GetLatest(assetId)
                : Channel.GetAssetInfo(assetId, version);

            _env = env;

            DisplayName = $".NET Core SDK {_assetInfo.Version}";
#if FEATURE_MULTI_ARCH_ASSETS
            DisplayName += " ({arch.ToString().ToLower()})";
#endif
        }

        public static string CreateAssetId(Architecture arch)
            => $"{AssetIdPrexix}.{PlatformConstants.RuntimeOSName}-{arch.ToString().ToLower()}";

        public override string DisplayName { get; }

        public override IEnumerable<Asset> Dependencies
            => _assetInfo
                .Dependencies
                .Select(d => new RuntimeAsset(Log, _env, d.Version, _arch));

        public override bool IsInstalled
            => _env.Sdks.Any(c => c.Version == _assetInfo.Version);

        public override bool Uninstall()
        {
            Log.Output($"Deleting files for {DisplayName}");
            var path = Path.Combine(_env.SdkRoot, _assetInfo.Version);
            return UninstallFolder(path);
        }

        public override async Task<bool> InstallAsync(CancellationToken cancellationToken)
        {
            Log.Verbose($"Begin installation of {DisplayName} to '{_env.Root}'");

            var url = _assetInfo.DownloadUrl;
            Log.Output($"Downloading {DisplayName}");
            if (!await DownloadAndExtractAsync(url, _env.Root, cancellationToken))
            {
                Log.Error($"Failed to install {DisplayName}");
                return false;
            }

            Log.Output($"Installed {DisplayName}");
            return true;
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

            var other = (SdkAsset)obj;
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
