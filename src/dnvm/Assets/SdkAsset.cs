using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.VersionManager.Files;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace DotNet.VersionManager.Assets
{
    public class SdkAsset : DotNetAssetBase
    {
        public const string DefaultVersion = "stable";
        private const string AssetIdPrexix = "Microsoft.DotNet.Cli";
        private readonly DotNetEnv _env;
        private readonly string _version;
        private readonly string _assetId;

        public SdkAsset(ILogger logger, DotNetEnv env, string version, Architecture arch)
            : base(logger)
        {
            _assetId = GetAssetId(arch);
            _version = version == DefaultVersion
                ? Channel.GetLatestVersion(_assetId)
                : version;
            _env = env;

            DisplayName = $".NET Core SDK {_version}";
#if FEATURE_MULTI_ARCH_ASSETS
            DisplayName += " ({arch.ToString().ToLower()})";
#endif
        }

        public static string GetAssetId(Architecture arch)
            => $"{AssetIdPrexix}.{PlatformConstants.RuntimeOSName}-{arch.ToString().ToLower()}";

        public override string DisplayName { get; }

        public override bool Uninstall()
        {
            Log.Output($"Uninstalling {DisplayName}");
            var path = Path.Combine(_env.SdkRoot, _version);
            return UninstallFolder(path);
        }

        public override async Task<bool> InstallAsync(CancellationToken cancellationToken)
        {
            Log.Output($"Installing {DisplayName}");

            if (_env.Sdks.Any(c => c.Version == _version))
            {
                Log.Verbose($"Skipping installation of {DisplayName}. Already installed.");
                Log.Output("Done");
                return true;
            }

            var url = Channel.GetDownloadUrl(_assetId, _version);
            Log.Output($"Downloading {DisplayName}");
            if (!await DownloadAndExtractAsync(url, _env.Root, cancellationToken))
            {
                Log.Error($"Failed to install {DisplayName}");
                return false;
            }

            Log.Output($"Installed {DisplayName}");
            return true;
        }
    }
}
