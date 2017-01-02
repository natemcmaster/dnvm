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
        public const string DefaultVersion = "latest";
        private const string AssetName = "Microsoft.NETCore.App";

        private readonly string _version;
        private readonly DotNetEnv _env;

        private static readonly HttpClient DefaultHttpClient = new HttpClient();

        public SharedFxAsset(IReporter reporter, DotNetEnv env, string version)
            : base(reporter)
        {
            _version = version == DefaultVersion
                ? Repo.GetLatestVersion(AssetName)
                : version;

            _env = env;
        }

        public override async Task<bool> InstallAsync(CancellationToken cancellationToken)
        {

            var assetFullName = $"{AssetName}@{_version}";
            Reporter.Output($"Installing '{assetFullName}'");
            Reporter.Verbose($"Begin installation of {assetFullName} to '{_env.Root}'");

            var dest = Path.Combine(_env.Root, "shared", AssetName, _version);
            Directory.CreateDirectory(dest);

#if __MACOS__
            var openssl = new OpenSslAsset(dest);
            Reporter.Verbose($"Linking OpenSSL from Homebrew into {assetFullName}");
            if (!await openssl.InstallAsync(cancellationToken))
            {
                Reporter.Warn($"Failed to install OpenSSL into {assetFullName}. Try running `brew install openssl` first and re-run this command.");
            }
#endif

            if (_env.Frameworks.Any(f => f.Name.Equals(AssetName, StringComparison.OrdinalIgnoreCase) && f.Version == _version))
            {
                Reporter.Verbose($"Skipping installation of {assetFullName}. Already installed.");
                return true;
            }

            var url = Repo.GetDownloadUrl(AssetName, _version);

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
    }
}