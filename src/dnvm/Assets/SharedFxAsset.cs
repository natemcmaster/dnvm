using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using DotNet.Files;
using DotNet.Reporting;
using System.IO;

namespace DotNet.Assets
{
    public class SharedFxAsset : DotNetAssetBase
    {
        public const string DefaultVersion = "stable";
        private const string AssetName = "Microsoft.NETCore.App";

        private readonly string _version;
        private readonly DotNetEnv _env;

        private static readonly HttpClient DefaultHttpClient = new HttpClient();

        public SharedFxAsset(IReporter reporter, DotNetEnv env, string version)
            : base(reporter)
        {
            _version = version == DefaultVersion
                ? Channel.GetLatestVersion(AssetName)
                : version;

            _env = env;
        }

        public override async Task<bool> InstallAsync(CancellationToken cancellationToken)
        {

            var assetFullName = $"{AssetName}@{_version}";
            Reporter.Output($"Installing '{assetFullName}'");
            Reporter.Verbose($"Begin installation of {assetFullName} to '{_env.Root}'");

            var dest = Path.Combine(_env.Root, "shared", AssetName, _version);

            if (_env.Frameworks.Any(f => f.Name.Equals(AssetName, StringComparison.OrdinalIgnoreCase) && f.Version == _version))
            {
                await EnsureAssetsLinkedIntoFramework(dest, cancellationToken);
                Reporter.Verbose($"Skipping installation of {assetFullName}. Already installed.");
                return true;
            }

            Directory.CreateDirectory(dest);

            await EnsureAssetsLinkedIntoFramework(dest, cancellationToken);

            var url = Channel.GetDownloadUrl(AssetName, _version);

            Reporter.Output($"Downloading {assetFullName}");
            if (!await DownloadAndExtractAsync(url, _env.Root, cancellationToken))
            {
                Reporter.Error($"Failed to install {assetFullName}");

                if (Directory.EnumerateFiles(dest).Any())
                {
                    try
                    {
                        Directory.Delete(dest, recursive: true);
                    }
                    catch
                    {
                        Reporter.Verbose($"Failed to delete {dest}");
                    }
                }

                return false;
            }

            return true;
        }

        private async Task EnsureAssetsLinkedIntoFramework(string dest, CancellationToken cancellationToken)
        {
#if __MACOS__
            var openssl = new OpenSslAsset(dest);
            Reporter.Verbose($"Linking OpenSSL from Homebrew into '{dest}'");
            if (!await openssl.InstallAsync(cancellationToken))
            {
                Reporter.Warn($"Failed to install OpenSSL into {AssetName}@{_version}. Try running `brew install openssl` first and re-run this command.");
            }
#endif
        }
    }
}