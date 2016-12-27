using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Files;
using DotNet.Reporting;

namespace DotNet.Assets
{
    public class SdkAsset : DotNetAssetBase
    {
        public const string Name = "DotNetCli";
        public const string DefaultVersion = "latest";

        // leading up to 1.0.0, the latest nightly is stored at https://dotnetcli.azureedge.net/dotnet/Sdk/rel-1.0.0/latest.version
        // this is the latest 'release'
        private const string LatestVersion = "1.0.0-preview4-004233";

        private readonly DotNetEnv _env;
        private readonly string _version;

        public SdkAsset(IReporter reporter, DotNetEnv env, string version)
            : base(reporter)
        {
            _version = version == DefaultVersion
                ? LatestVersion
                : version;
            _env = env;
        }

        public override async Task<bool> InstallAsync(CancellationToken cancellationToken)
        {
            Reporter.Output($"Installing .NET Core CLI {_version}");
            if (_env.Clis.Any(c => c.Version == _version))
            {
                Reporter.Verbose($"Skipping installation of CLI {_version}. Already installed.");
                return true;
            }

            var url = CreateDownloadUrl(_version);
            Reporter.Output($"Downloading .NET Core CLI {_version}");
            if (!await DownloadAndExtractAsync(url, _env.Root, cancellationToken))
            {
                Reporter.Error($"Failed to install CLI {_version}");
                return false;
            }

            Reporter.Output("Installed");
            return true;
        }

        internal static string CreateDownloadUrl(string version)
        {
            if (version.StartsWith("1.0.0-preview2"))
            {
                return $"{AzureFeed}/preview/Binaries/{version}/dotnet-dev-{GetRid()}.{version}.tar.gz";
            }

            return $"{AzureFeed}/Sdk/{version}/dotnet-dev-{GetRid()}.{version}.tar.gz";
        }
    }
}