using System.Linq;
using DotNet.VersionManager.Assets;
using FluentAssertions;
using Xunit;

namespace DotNet.VersionManager.Tests
{
    public class AssetChannelTest
    {
        [Theory]
        [InlineData("1.0.0", "https://dotnetcli.azureedge.net/dotnet/preview/Binaries/1.0.0/dotnet-osx-x64.1.0.0.tar.gz")]
        [InlineData("1.0.1", "https://dotnetcli.azureedge.net/dotnet/preview/Binaries/1.0.1/dotnet-osx-x64.1.0.1.tar.gz")]
        [InlineData("1.0.3", "https://dotnetcli.azureedge.net/dotnet/preview/Binaries/1.0.3/dotnet-osx-x64.1.0.3.tar.gz")]
        [InlineData("1.0.4", "https://dotnetcli.azureedge.net/dotnet/preview/Binaries/1.0.4/dotnet-osx-x64.1.0.4.tar.gz")]
        [InlineData("1.1.0", "https://dotnetcli.azureedge.net/dotnet/release/1.1.0/Binaries/1.1.0/dotnet-osx-x64.1.1.0.tar.gz")]
        [InlineData("1.1.1", "https://dotnetcli.azureedge.net/dotnet/release/1.1.0/Binaries/1.1.1/dotnet-osx-x64.1.1.1.tar.gz")]
        [InlineData("1.1.6", "https://dotnetcli.azureedge.net/dotnet/release/1.1.0/Binaries/1.1.6/dotnet-osx-x64.1.1.6.tar.gz")]
        [InlineData("2.0.0", "https://dotnetcli.azureedge.net/dotnet/Runtime/2.0.0/dotnet-runtime-2.0.0-osx-x64.tar.gz")]
        public void CreatesSharedFxDownloadUrl(string version, string expectedUrl)
        {
            new StableAssetChannel()
                .GetAssetInfo("Microsoft.NETCore.App.osx-x64", version)
                .DownloadUrl
                .Should()
                .Be(expectedUrl);
        }

        [Theory]
        [InlineData("1.0.0", "https://dotnetcli.azureedge.net/dotnet/Sdk/1.0.0/dotnet-dev-osx-x64.1.0.0.tar.gz")]
        [InlineData("1.0.1", "https://dotnetcli.azureedge.net/dotnet/Sdk/1.0.1/dotnet-dev-osx-x64.1.0.1.tar.gz")]
        [InlineData("1.1.7", "https://dotnetcli.azureedge.net/dotnet/Sdk/1.1.7/dotnet-dev-osx-x64.1.1.7.tar.gz")]
        [InlineData("2.0.0", "https://dotnetcli.azureedge.net/dotnet/Sdk/2.0.0/dotnet-sdk-2.0.0-osx-x64.tar.gz")]
        public void CreatesCliDownloadUrl(string version, string expectedUrl)
        {
            new StableAssetChannel()
                .GetAssetInfo("Microsoft.DotNet.Cli.osx-x64", version)
                .DownloadUrl
                .Should()
                .Be(expectedUrl);
        }

        [Theory]
        [InlineData("Microsoft.NETCore.App.osx-x64", "2.0.5")]
        [InlineData("Microsoft.DotNet.Cli.osx-x64", "2.1.4")]
        public void GetsLatestVersion(string assetId, string version)
        {
            new StableAssetChannel()
                .GetLatest(assetId)
                .Version
                .Should()
                .Be(version);
        }

        [Theory]
        [InlineData("1.0.1", new[] { "1.0.4", "1.1.1" })]
        [InlineData("2.1.4", new[] { "2.0.5" })]
        public void SdkHasDependenciesOn(string sdkVersion, string[] runtimeVersions)
        {
            new StableAssetChannel()
                .GetAssetInfo("Microsoft.DotNet.Cli.osx-x64", sdkVersion)
                .Dependencies
                .Select(d => d.Version)
                .Should().Contain(runtimeVersions);
        }
    }
}
