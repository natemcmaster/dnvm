using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Files;
using DotNet.Reporting;
using System.Runtime.InteropServices;

namespace DotNet.Assets
{
    public class SdkAsset : DotNetAssetBase
    {
        public const string DefaultVersion = "stable";
        private const string AssetIdPrexix = "Microsoft.DotNet.Cli";
        private readonly DotNetEnv _env;
        private readonly string _version;
        private readonly string _assetId;

        public SdkAsset(IReporter reporter, DotNetEnv env, string version, Architecture arch)
            : base(reporter)
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
            var path = Path.Combine(_env.SdkRoot, _version);
            return UninstallFolder(path);
        }

        public override async Task<bool> InstallAsync(CancellationToken cancellationToken)
        {
            Reporter.Output($"Installing {DisplayName}");

            if (_env.Sdks.Any(c => c.Version == _version))
            {
                Reporter.Verbose($"Skipping installation of {DisplayName}. Already installed.");
                Reporter.Output("Done");
                return true;
            }

            var url = Channel.GetDownloadUrl(_assetId, _version);
            Reporter.Output($"Downloading {DisplayName}");
            if (!await DownloadAndExtractAsync(url, _env.Root, cancellationToken))
            {
                Reporter.Error($"Failed to install {DisplayName}");
                return false;
            }

            Reporter.Output($"Installed {DisplayName}");
            return true;
        }
    }
}