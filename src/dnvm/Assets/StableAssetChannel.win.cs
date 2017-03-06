using System;
using System.Collections.Generic;

namespace DotNet.VersionManager.Assets
{
    // TODO make this info available remotely so it can be updated without re-installing dnvm
    partial class StableAssetChannel : IAssetChannel
    {
        private const string AzureUrl = "https://dotnetcli.blob.core.windows.net/dotnet";
        private IDictionary<string, AssetInfo> _assets
            = new Dictionary<string, AssetInfo>(StringComparer.OrdinalIgnoreCase)
            {
                ["Microsoft.NETCore.App.win-x64"] = new AssetInfo
                {
                    Versions =
                    {
                        ["1.0.0"] = $"{AzureUrl}/preview/Binaries/1.0.0/dotnet-win-x64.1.0.0.zip",
                        ["1.0.1"] = $"{AzureUrl}/preview/Binaries/1.0.1/dotnet-win-x64.1.0.1.zip",
                        ["1.0.3"] = $"{AzureUrl}/preview/Binaries/1.0.3/dotnet-win-x64.1.0.3.zip",
                        ["1.1.0"] = $"{AzureUrl}/release/1.1.0/Binaries/1.1.0/dotnet-win-x64.1.1.0.zip",
                    }
                },
                ["Microsoft.DotNet.Cli.win-x64"] = new AssetInfo
                {
                    Versions =
                    {
                        ["1.0.0-preview2-003121"] = $"{AzureUrl}/preview/Binaries/1.0.0-preview2-003121/dotnet-dev-win-x64.1.0.0-preview2-003121.zip",
                        ["1.0.0-preview2-003131"] = $"{AzureUrl}/preview/Binaries/1.0.0-preview2-003131/dotnet-dev-win-x64.1.0.0-preview2-003131.zip",
                        ["1.0.0-preview2-003156"] = $"{AzureUrl}/preview/Binaries/1.0.0-preview2-003156/dotnet-dev-win-x64.1.0.0-preview2-003156.zip",
                        ["1.0.0-preview2-1-003177"] = $"{AzureUrl}/preview/Binaries/1.0.0-preview2-1-003177/dotnet-dev-win-x64.1.0.0-preview2-1-003177.zip",
                        ["1.0.0-preview3-004056"] = $"{AzureUrl}/Sdk/1.0.0-preview3-004056/dotnet-dev-win-x64.1.0.0-preview3-004056.zip",
                        ["1.0.0-preview4-004233"] = $"{AzureUrl}/Sdk/1.0.0-preview4-004233/dotnet-dev-win-x64.1.0.0-preview4-004233.zip",
                        ["1.0.0-rc3-004530"] = $"{AzureUrl}/Sdk/1.0.0-rc3-004530/dotnet-dev-win-x64.1.0.0-rc3-004530.zip",
                    }
                },
            };
    }
}
