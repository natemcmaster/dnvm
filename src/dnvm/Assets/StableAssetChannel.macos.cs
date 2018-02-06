using System;
using System.Collections.Generic;

namespace DotNet.VersionManager.Assets
{
    // TODO make this info available remotely so it can be updated without re-installing dnvm
    partial class StableAssetChannel : IAssetChannel
    {
        public class MacOSRuntimeAssetInfo : AssetInfo
        {
            public MacOSRuntimeAssetInfo(string version, string channel)
                : this(version, legacy: true)
            {
                DownloadUrl = $"https://dotnetcli.azureedge.net/dotnet/{channel}/Binaries/{version}/dotnet-osx-x64.{version}.tar.gz";
            }

            public MacOSRuntimeAssetInfo(string version)
                : this(version, legacy: false)
            {
                DownloadUrl = $"https://dotnetcli.azureedge.net/dotnet/Runtime/{version}/dotnet-runtime-{version}-osx-x64.tar.gz";
            }

            private MacOSRuntimeAssetInfo(string version, bool legacy)
            {
                Version = version;
                Id = "Microsoft.NETCore.App.osx-x64";
                RequiresOpenSSL = legacy;
            }

            public bool RequiresOpenSSL { get; }
        }

        private class MacOSSdkAssetInfo : AssetInfo
        {
            public MacOSSdkAssetInfo(string version, MacOSRuntimeAssetInfo[] runtimes)
                : this(version, false, runtimes)
            {
            }

            public MacOSSdkAssetInfo(string version, bool legacy, MacOSRuntimeAssetInfo[] runtimes)
            {
                Version = version;
                Id = "Microsoft.DotNet.Cli.osx-x64";
                DownloadUrl = legacy
                    ? $"https://dotnetcli.azureedge.net/dotnet/Sdk/{version}/dotnet-dev-osx-x64.{version}.tar.gz"
                    : $"https://dotnetcli.azureedge.net/dotnet/Sdk/{version}/dotnet-sdk-{version}-osx-x64.tar.gz";

                runtimes = runtimes ?? Array.Empty<MacOSRuntimeAssetInfo>();
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
                    new MacOSRuntimeAssetInfo("1.0.0", "preview"),
                    new MacOSRuntimeAssetInfo("1.0.1", "preview"),
                    new MacOSRuntimeAssetInfo("1.0.3", "preview"),
                    new MacOSRuntimeAssetInfo("1.0.4", "preview"),
                    new MacOSRuntimeAssetInfo("1.0.5", "preview"),
                    new MacOSRuntimeAssetInfo("1.0.7", "preview"),
                    new MacOSRuntimeAssetInfo("1.0.8", "preview"),
                    new MacOSRuntimeAssetInfo("1.0.9", "preview"),
                    new MacOSRuntimeAssetInfo("1.1.0", "release/1.1.0"),
                    new MacOSRuntimeAssetInfo("1.1.1", "release/1.1.0"),
                    new MacOSRuntimeAssetInfo("1.1.2", "release/1.1.0"),
                    new MacOSRuntimeAssetInfo("1.1.4", "release/1.1.0"),
                    new MacOSRuntimeAssetInfo("1.1.5", "release/1.1.0"),
                    new MacOSRuntimeAssetInfo("1.1.6", "release/1.1.0"),
                    new MacOSRuntimeAssetInfo("2.0.0"),
                    new MacOSRuntimeAssetInfo("2.0.3"),
                    new MacOSRuntimeAssetInfo("2.0.4"),
                    new MacOSRuntimeAssetInfo("2.0.5"),
                },
                ["Microsoft.DotNet.Cli.osx-x64"] = new List<AssetInfo>
                {
                    new MacOSSdkAssetInfo("1.0.0", true,
                        new[]
                        {
                            new MacOSRuntimeAssetInfo("1.0.4", "preview"),
                            new MacOSRuntimeAssetInfo("1.1.1", "release/1.1.0")
                        }),
                    new MacOSSdkAssetInfo("1.0.1", true,
                        new[]
                        {
                            new MacOSRuntimeAssetInfo("1.0.4", "preview"),
                            new MacOSRuntimeAssetInfo("1.1.1", "release/1.1.0")
                        }),
                    new MacOSSdkAssetInfo("1.1.7", true,
                        new[]
                        {
                            new MacOSRuntimeAssetInfo("1.0.9", "preview"),
                            new MacOSRuntimeAssetInfo("1.1.6", "release/1.1.0")
                        }),
                    new MacOSSdkAssetInfo("2.0.0",
                        new[]
                        {
                            new MacOSRuntimeAssetInfo("2.0.0"),
                        }),
                    new MacOSSdkAssetInfo("2.0.1",
                        new[]
                        {
                            new MacOSRuntimeAssetInfo("2.0.0"),
                        }),
                    new MacOSSdkAssetInfo("2.1.2",
                        new[]
                        {
                            new MacOSRuntimeAssetInfo("2.0.3"),
                        }),
                    new MacOSSdkAssetInfo("2.1.3",
                        new[]
                        {
                            new MacOSRuntimeAssetInfo("2.0.4"),
                        }),
                    new MacOSSdkAssetInfo("2.1.4",
                        new[]
                        {
                            new MacOSRuntimeAssetInfo("2.0.5"),
                        }),
                },
            };
    }
}
