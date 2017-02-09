using System;
using System.Collections.Generic;

namespace DotNet.Assets
{
    // TODO make this info available remotely so it can be updated without re-installing dnvm
    partial class StableAssetChannel : IAssetChannel
    {
        private const string AzureUrl = "https://dotnetcli.blob.core.windows.net/dotnet";

        private IDictionary<string, AssetInfo> _assets
            = new Dictionary<string, AssetInfo>(StringComparer.OrdinalIgnoreCase)
            {
                ["Microsoft.NETCore.App.osx-x64"] = new AssetInfo
                {
                    Versions =
                    {
                        ["1.0.0"] = $"{AzureUrl}/preview/Binaries/1.0.0/dotnet-osx-x64.1.0.0.tar.gz",
                        ["1.0.1"] = $"{AzureUrl}/preview/Binaries/1.0.1/dotnet-osx-x64.1.0.1.tar.gz",
                        ["1.0.3"] = $"{AzureUrl}/preview/Binaries/1.0.3/dotnet-osx-x64.1.0.3.tar.gz",
                        ["1.1.0"] = $"{AzureUrl}/release/1.1.0/Binaries/1.1.0/dotnet-osx-x64.1.1.0.tar.gz",
                    }
                },
                ["Microsoft.DotNet.Cli.osx-x64"] = new AssetInfo
                {
                    Versions =
                    {
                        ["1.0.0-preview2-003121"] = $"{AzureUrl}/preview/Binaries/1.0.0-preview2-003121/dotnet-dev-osx-x64.1.0.0-preview2-003121.tar.gz",
                        ["1.0.0-preview2-003131"] = $"{AzureUrl}/preview/Binaries/1.0.0-preview2-003131/dotnet-dev-osx-x64.1.0.0-preview2-003131.tar.gz",
                        ["1.0.0-preview2-003156"] = $"{AzureUrl}/preview/Binaries/1.0.0-preview2-003156/dotnet-dev-osx-x64.1.0.0-preview2-003156.tar.gz",
                        ["1.0.0-preview2-1-003177"] = $"{AzureUrl}/preview/Binaries/1.0.0-preview2-1-003177/dotnet-dev-osx-x64.1.0.0-preview2-1-003177.tar.gz",
                        ["1.0.0-preview3-004056"] = $"{AzureUrl}/Sdk/1.0.0-preview3-004056/dotnet-dev-osx-x64.1.0.0-preview3-004056.tar.gz",
                        ["1.0.0-preview4-004233"] = $"{AzureUrl}/Sdk/1.0.0-preview4-004233/dotnet-dev-osx-x64.1.0.0-preview4-004233.tar.gz",
                        ["1.0.0-rc3-004530"] = $"{AzureUrl}/Sdk/1.0.0-rc3-004530/dotnet-dev-osx-x64.1.0.0-rc3-004530.tar.gz",
                        ["1.0.0-rc4-004771"] = $"{AzureUrl}/Sdk/1.0.0-rc4-004771/dotnet-dev-osx-x64.1.0.0-rc4-004771.tar.gz",
                    }
                },
            };
    }
}