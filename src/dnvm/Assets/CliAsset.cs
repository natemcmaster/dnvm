using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Assets
{
    public class CliAsset : Asset
    {
        public const string Name = "DotNetCli";
        public const string DefaultVersion = "latest";

        private const string AzureFeed = "https://dotnetcli.azureedge.net/dotnet";

        // leading up to 1.0.0, the latest nightly is stored at https://dotnetcli.azureedge.net/dotnet/Sdk/rel-1.0.0/latest.version
        // this is the latest 'release'
        private const string LatestVersion = "1.0.0-preview4-004233";

        private readonly string _version;

        public CliAsset(string version)
        {
            _version = version == DefaultVersion
                ? LatestVersion
                : version;
        }

        public override Task InstallAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private static string CreateDownloadUrl(string version, string channel)
        {
            const string arch = "x64";
            const string osname = "osx";
            return $"{AzureFeed}/Sdk/{version}/dotnet-dev-{osname}-{arch}.{version}.tar.gz";
        }
    }
}