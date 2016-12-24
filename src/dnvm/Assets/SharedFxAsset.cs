using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Utils;
using NuGet.Versioning;

namespace DotNet.Assets
{
    using System.Linq;
    using DotNet.Reporting;
    using Environment = DotNet.Files.Environment;

    public class SharedFxAsset : Asset
    {
        public const string DefaultVersion = "latest";
        public const string Name = "Microsoft.NETCore.App";

        const string AzureFeed = "https://dotnetcli.blob.core.windows.net/dotnet";
        const string LatestVersion = "1.1.0";

        private readonly string _version;
        private readonly Environment _env;
        private readonly IReporter _reporter;

        private static readonly HttpClient DefaultHttpClient = new HttpClient();

        public SharedFxAsset(IReporter reporter, Environment env, string version)
        {
            _version = version == DefaultVersion
                ? LatestVersion
                : version;
                
            _reporter = reporter;
            _env = env;
        }

        public override async Task InstallAsync(CancellationToken cancellationToken)
        {
            _reporter.Verbose($"Begin installation of {Name}@{_version} to '{_env.Root}'");
            
            if (_env.Frameworks.Any(f => f.Name.Equals(Name, StringComparison.OrdinalIgnoreCase) && f.Version == _version))
            {
                _reporter.Verbose($"Skipping installation of {Name}@{_version}. Already installed.");
                return;
            }

            var url = CreateDownloadUrl(_version);

            _reporter.Verbose($"Downloading '{url}'");
            
            var result = await DefaultHttpClient.GetAsync(url, cancellationToken);
            if (!result.IsSuccessStatusCode)
            {
                _reporter.Error($"Failed to download '{url}'");
                throw new InvalidOperationException("Installation failed");
            }

            var filename = Path.GetFileName(url);
            bool success;
            using (var tmp = new TempFile())
            using (var stream = new FileStream(tmp.Path, FileMode.Create))
            {
                await result.Content.CopyToAsync(stream);

                _reporter.Verbose($"Extracting '{filename}' to '{_env.Root}'");
                success = TarballExtractor.Extract(tmp.Path, _env.Root, gzipped: true);
            }

            if (success)
            {
                _reporter.Output($"Installed {Name}@{_version}");
            }
            else
            {
                _reporter.Error($"Failed to install {Name}@{_version}");
                throw new InvalidOperationException("Installation failed");
            }
        }

        public static string CreateDownloadUrl(string version)
        {
            const string arch = "x64";
            const string osname = "osx";
            var channel = MapChannel(version);

            return $"{AzureFeed}/{channel}/Binaries/{version}/dotnet-{osname}-{arch}.{version}.tar.gz";
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