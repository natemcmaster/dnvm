using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using DotNet.Files;
using DotNet.Reporting;
using System.Runtime.InteropServices;

namespace DotNet.Assets
{
    public class SharedFxAsset : DotNetAssetBase
    {
        public const string DefaultVersion = "stable";
        public const string AssetIdPrefix = "Microsoft.NETCore.App";

        private readonly string _version;
        private readonly DotNetEnv _env;
        private readonly string _assetId;

        private static readonly HttpClient DefaultHttpClient = new HttpClient();

        public SharedFxAsset(IReporter reporter, DotNetEnv env, string version, Architecture arch)
            : base(reporter)
        {
            _assetId = GetAssetId(arch);
            _version = version == DefaultVersion
                ? Channel.GetLatestVersion(_assetId)
                : version;

            _env = env;
            DisplayName = $"{_assetId} {_version}";
#if FEATURE_MULTI_ARCH_ASSETS
            DisplayName += " ({arch.ToString().ToLower()})";
#endif
        }

        public static string GetAssetId(Architecture arch)
            => $"{AssetIdPrefix}.{PlatformConstants.RuntimeOSName}-{arch.ToString().ToLower()}";

        public override string DisplayName { get; }

        public override bool Uninstall()
        {
            var path = Path.Combine(_env.FxRoot, _assetId, _version);
            return UninstallFolder(path);
        }

        public override async Task<bool> InstallAsync(CancellationToken cancellationToken)
        {

            var assetFullName = $"{_assetId}@{_version}";
            Reporter.Output($"Installing '{assetFullName}'");
            Reporter.Verbose($"Begin installation of {assetFullName} to '{_env.Root}'");

            var dest = Path.Combine(_env.FxRoot, _assetId, _version);

            if (_env.Frameworks.Any(f => f.Name.Equals(_assetId, StringComparison.OrdinalIgnoreCase) && f.Version == _version))
            {
                await EnsureAssetsLinkedIntoFramework(dest, cancellationToken);
                Reporter.Verbose($"Skipping installation of {assetFullName}. Already installed.");
                return true;
            }

            Directory.CreateDirectory(dest);

            await EnsureAssetsLinkedIntoFramework(dest, cancellationToken);

            var url = Channel.GetDownloadUrl(_assetId, _version);

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
                Reporter.Warn($"Failed to install OpenSSL into {DisplayName}. Try running `brew install openssl` first and re-run this command.");
            }
#else
            await Task.CompletedTask;
#endif
        }
    }
}
