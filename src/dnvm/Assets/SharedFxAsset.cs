using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using DotNet.Reporting;
using NuGet.Versioning;
using DotNetEnv = DotNet.Files.DotNetEnv;

namespace DotNet.Assets
{
    public class SharedFxAsset : DotNetAssetBase
    {
        public const string DefaultVersion = "latest";
        public const string Name = "Microsoft.NETCore.App";

        const string LatestVersion = "1.1.0";

        private readonly string _assetFullName;
        private readonly string _version;
        private readonly DotNetEnv _env;

        private static readonly HttpClient DefaultHttpClient = new HttpClient();

        public SharedFxAsset(IReporter reporter, DotNetEnv env, string version)
            : base(reporter)
        {
            _version = version == DefaultVersion
                ? LatestVersion
                : version;

            _env = env;
            _assetFullName = $"{Name}@{_version}";
        }

        public override async Task<bool> InstallAsync(CancellationToken cancellationToken)
        {
            Reporter.Output($"Installing '{_assetFullName}'");
            Reporter.Verbose($"Begin installation of {_assetFullName} to '{_env.Root}'");

            if (_env.Frameworks.Any(f => f.Name.Equals(Name, StringComparison.OrdinalIgnoreCase) && f.Version == _version))
            {
                Reporter.Verbose($"Skipping installation of {_assetFullName}. Already installed.");
                return true;
            }

            var url = CreateDownloadUrl(_version);

            Reporter.Output($"Downloading {_assetFullName}");
            if (!await DownloadAndExtractAsync(url, _env.Root, cancellationToken))
            {
                Reporter.Error($"Failed to install {_assetFullName}");
                return false;
            }

            return true;
        }

        public static string CreateDownloadUrl(string version)
        {
            var channel = MapChannel(version);

            return $"{AzureFeed}/{channel}/Binaries/{version}/dotnet-{GetRid()}.{version}.tar.gz";
        }

        private static string MapChannel(string version)
        {
            const string Default = "master";
            if (!SemanticVersion.TryParse(version, out var semver))
            {
                return Default;
            }

            switch (semver.Major)
            {
                case 1:
                    switch (semver.Minor)
                    {
                        case 0:
                            return "preview";
                        case 1:
                            return "release/1.1.0";
                    }
                    goto default;
                default:
                    return Default;
            }
        }
    }
}