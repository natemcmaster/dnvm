using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using DotNet.Files;
using DotNet.Reporting;

namespace DotNet.Assets
{
    public class SharedFxAsset : DotNetAssetBase
    {
        public const string DefaultVersion = "stable";
        public const string AssetId = "Microsoft.NETCore.App";

        private readonly string _version;
        private readonly DotNetEnv _env;

        private static readonly HttpClient DefaultHttpClient = new HttpClient();


        public SharedFxAsset(IReporter reporter, DotNetEnv env, string version)
            : base(reporter)
        {
            _version = version == DefaultVersion
                ? Channel.GetLatestVersion(AssetId)
                : version;

            _env = env;
            DisplayName = $"{AssetId} {_version}";
        }

        public override string DisplayName { get; }

        public override bool Uninstall()
        {
            var path = Path.Combine(_env.FxRoot, AssetId, _version);
            return UninstallFolder(path);
        }

        public override async Task<bool> InstallAsync(CancellationToken cancellationToken)
        {

            var assetFullName = $"{AssetId}@{_version}";
            Reporter.Output($"Installing '{assetFullName}'");
            Reporter.Verbose($"Begin installation of {assetFullName} to '{_env.Root}'");

            var dest = Path.Combine(_env.FxRoot, AssetId, _version);

            if (_env.Frameworks.Any(f => f.Name.Equals(AssetId, StringComparison.OrdinalIgnoreCase) && f.Version == _version))
            {
                await EnsureAssetsLinkedIntoFramework(dest, cancellationToken);
                Reporter.Verbose($"Skipping installation of {assetFullName}. Already installed.");
                return true;
            }

            Directory.CreateDirectory(dest);

            await EnsureAssetsLinkedIntoFramework(dest, cancellationToken);

            var url = Channel.GetDownloadUrl(AssetId, _version);

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
                Reporter.Warn($"Failed to install OpenSSL into {AssetId}@{_version}. Try running `brew install openssl` first and re-run this command.");
            }
#endif
        }
    }
}