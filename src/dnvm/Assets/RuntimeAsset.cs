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
    public class RuntimeAsset : DotNetAssetBase
    {
        public const string DefaultVersion = "stable";
        public const string AssetIdPrefix = "Microsoft.NETCore.App";

        private readonly string _version;
        private readonly DotNetEnv _env;
        private readonly string _assetId;

        private static readonly HttpClient DefaultHttpClient = new HttpClient();

        public RuntimeAsset(ILogger logger, DotNetEnv env, string version, Architecture arch)
            : base(logger)
        {
            _assetId = GetAssetId(arch);
            _version = version == DefaultVersion
                ? Channel.GetLatestVersion(_assetId)
                : version;

            _env = env;
            DisplayName = $".NET Core Runtime {_version}";
#if FEATURE_MULTI_ARCH_ASSETS
            DisplayName += " ({arch.ToString().ToLower()})";
#endif
        }

        public static string GetAssetId(Architecture arch)
            => $"{AssetIdPrefix}.{PlatformConstants.RuntimeOSName}-{arch.ToString().ToLower()}";

        public override string DisplayName { get; }

        public override bool Uninstall()
        {
            Log.Output($"Uninstalling {DisplayName}");
            return UninstallFolder(GetInstallationPath());
        }

        public override async Task<bool> InstallAsync(CancellationToken cancellationToken)
        {
            Log.Output($"Installing {DisplayName}");
            Log.Verbose($"Begin installation of {DisplayName} to '{_env.Root}'");

            var dest = GetInstallationPath();

            if (_env.Runtimes.Any(f => f.Name.Equals(_assetId, StringComparison.OrdinalIgnoreCase) && f.Version == _version))
            {
                await EnsureRuntimeDependencies(dest, cancellationToken);
                Log.Verbose($"Skipping installation of {DisplayName}. Already installed.");
                return true;
            }

            Directory.CreateDirectory(dest);

            await EnsureRuntimeDependencies(dest, cancellationToken);

            var url = Channel.GetDownloadUrl(_assetId, _version);

            Log.Output($"Downloading {DisplayName}");
            if (!await DownloadAndExtractAsync(url, _env.Root, cancellationToken))
            {
                Log.Error($"Failed to install {DisplayName}");

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

            Log.Output($"Installed {DisplayName}");

            return true;
        }

        private string GetInstallationPath()
            => Path.Combine(_env.FxRoot, "Microsoft.NETCore.App", _version);

        private async Task EnsureRuntimeDependencies(string dest, CancellationToken cancellationToken)
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
