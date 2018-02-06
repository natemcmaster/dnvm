using System;
using System.Collections.Generic;

namespace DotNet.VersionManager.Assets
{
    // TODO make this info available remotely so it can be updated without re-installing dnvm
    partial class StableAssetChannel : IAssetChannel
    {
        private class RuntimeAssetInfo : AssetInfo
        {
            public RuntimeAssetInfo(string version, string channel)
            {
                Version = version;
                Id = "Microsoft.NETCore.App.osx-x64";
                DownloadUrl = $"https://dotnetcli.azureedge.net/dotnet/{channel}/Binaries/{version}/dotnet-osx-x64.{version}.tar.gz";
            }
        }

        private class SdkAssetInfo : AssetInfo
        {
            public SdkAssetInfo(string version, string folder, RuntimeAssetInfo[] runtimes)
            {
                Version = version;
                Id = "Microsoft.DotNet.Cli.osx-x64";
                DownloadUrl = $"https://dotnetcli.azureedge.net/dotnet/{folder}/{version}/dotnet-dev-osx-x64.{version}.tar.gz";

                runtimes = runtimes ?? Array.Empty<RuntimeAssetInfo>();
                foreach (var runtime in runtimes)
                {
                    Dependencies.Add(runtime);
                }
            }
        }

        private IDictionary<string, ICollection<AssetInfo>> _assets
            = new Dictionary<string, ICollection<AssetInfo>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Microsoft.NETCore.App.osx-x64"] = new List<AssetInfo>
                {
                    new RuntimeAssetInfo("1.0.0", "preview"),
                    new RuntimeAssetInfo("1.0.1", "preview"),
                    new RuntimeAssetInfo("1.0.3", "preview"),
                    new RuntimeAssetInfo("1.0.4", "preview"),
                    new RuntimeAssetInfo("1.0.5", "preview"),
                    new RuntimeAssetInfo("1.0.7", "preview"),
                    new RuntimeAssetInfo("1.0.8", "preview"),
                    new RuntimeAssetInfo("1.0.9", "preview"),
                    new RuntimeAssetInfo("1.1.0", "release/1.1.0"),
                    new RuntimeAssetInfo("1.1.1", "release/1.1.0"),
                    new RuntimeAssetInfo("1.1.2", "release/1.1.0"),
                    new RuntimeAssetInfo("1.1.4", "release/1.1.0"),
                    new RuntimeAssetInfo("1.1.5", "release/1.1.0"),
                    new RuntimeAssetInfo("1.1.6", "release/1.1.0"),
                },
                ["Microsoft.DotNet.Cli.osx-x64"] = new List<AssetInfo>
                {
                    new SdkAssetInfo("1.0.0", "Sdk",
                        new[]
                        {
                            new RuntimeAssetInfo("1.0.4", "preview"),
                            new RuntimeAssetInfo("1.1.1", "release/1.1.0")
                        }),
                    new SdkAssetInfo("1.0.1", "Sdk",
                        new[]
                        {
                            new RuntimeAssetInfo("1.0.4", "preview"),
                            new RuntimeAssetInfo("1.1.1", "release/1.1.0")
                        }),
                    new SdkAssetInfo("1.1.7", "Sdk",
                        new[]
                        {
                            new RuntimeAssetInfo("1.0.9", "preview"),
                            new RuntimeAssetInfo("1.1.6", "release/1.1.0")
                        }),
                },
            };
    }
}
