using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using DotNet.VersionManager.Files;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace DotNet.VersionManager.Assets
{
    public class SharedFxAsset : DotNetAssetBase
    {
        public const string DefaultVersion = "stable";
        public const string AssetIdPrefix = "Microsoft.NETCore.App";

        private readonly string _version;
        private readonly DotNetEnv _env;
        private readonly string _assetId;

        private static readonly HttpClient DefaultHttpClient = new HttpClient();

        public SharedFxAsset(ILogger logger, DotNetEnv env, string version, Architecture arch)
            : base(logger)
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
            Log.Output($"Installing '{assetFullName}'");
            Log.Verbose($"Begin installation of {assetFullName} to '{_env.Root}'");

            var dest = Path.Combine(_env.FxRoot, _assetId, _version);

            if (_env.Frameworks.Any(f => f.Name.Equals(_assetId, StringComparison.OrdinalIgnoreCase) && f.Version == _version))
            {
                await EnsureAssetsLinkedIntoFramework(dest, cancellationToken);
                Log.Verbose($"Skipping installation of {assetFullName}. Already installed.");
                return true;
            }

            Directory.CreateDirectory(dest);

            await EnsureAssetsLinkedIntoFramework(dest, cancellationToken);

            var url = Channel.GetDownloadUrl(_assetId, _version);

            Log.Output($"Downloading {assetFullName}");
            if (!await DownloadAndExtractAsync(url, _env.Root, cancellationToken))
            {
                Log.Error($"Failed to install {assetFullName}");

                if (Directory.EnumerateFiles(dest).Any())
                {
                    try
                    {
                        Directory.Delete(dest, recursive: true);
                    }
                    catch
                    {
                        Log.Verbose($"Failed to delete {dest}");
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
            Log.Verbose($"Linking OpenSSL from Homebrew into '{dest}'");
            if (!await openssl.InstallAsync(cancellationToken))
            {
                Log.Warn($"Failed to install OpenSSL into {DisplayName}. Try running `brew install openssl` first and re-run this command.");
            }
#else
            await Task.CompletedTask;
#endif
        }
    }
}
