using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Files;
using DotNet.Reporting;

namespace DotNet.Assets
{
    public class SdkAsset : DotNetAssetBase
    {
        public const string DefaultVersion = "stable";
        public const string AssetId = "Microsoft.DotNet.Cli.osx-x64";
        private readonly DotNetEnv _env;
        private readonly string _version;

        public SdkAsset(IReporter reporter, DotNetEnv env, string version)
            : base(reporter)
        {
            _version = version == DefaultVersion
                ? Channel.GetLatestVersion(AssetId)
                : version;
            _env = env;
            DisplayName = $".NET Core SDK {_version}";
        }

        public override string DisplayName { get; }

        public override bool Uninstall()
        {
            var path = Path.Combine(_env.Root, "sdk", _version);
            return UninstallFolder(path);
        }

        public override async Task<bool> InstallAsync(CancellationToken cancellationToken)
        {
            Reporter.Output($"Installing .NET Core CLI {_version}");
            if (_env.Clis.Any(c => c.Version == _version))
            {
                Reporter.Verbose($"Skipping installation of CLI {_version}. Already installed.");
                return true;
            }

            var url = Channel.GetDownloadUrl(AssetId, _version);
            Reporter.Output($"Downloading .NET Core CLI {_version}");
            if (!await DownloadAndExtractAsync(url, _env.Root, cancellationToken))
            {
                Reporter.Error($"Failed to install CLI {_version}");
                return false;
            }

            Reporter.Output("Installed");
            return true;
        }
    }
}