using DotNet.Assets;
using FluentAssertions;
using Xunit;

namespace DotNet.Test
{
    public class AssetChannelTest
    {
        [Theory]
        [InlineData("1.0.0", "https://dotnetcli.blob.core.windows.net/dotnet/preview/Binaries/1.0.0/dotnet-osx-x64.1.0.0.tar.gz")]
        [InlineData("1.0.1", "https://dotnetcli.blob.core.windows.net/dotnet/preview/Binaries/1.0.1/dotnet-osx-x64.1.0.1.tar.gz")]
        [InlineData("1.0.3", "https://dotnetcli.blob.core.windows.net/dotnet/preview/Binaries/1.0.3/dotnet-osx-x64.1.0.3.tar.gz")]
        [InlineData("1.1.0", "https://dotnetcli.blob.core.windows.net/dotnet/release/1.1.0/Binaries/1.1.0/dotnet-osx-x64.1.1.0.tar.gz")]
        public void CreatesSharedFxDownloadUrl(string version, string expectedUrl)
        {
            new StableAssetChannel()
                .GetDownloadUrl("Microsoft.NETCore.App", version)
                .Should()
                .Be(expectedUrl);
        }

        [Theory]
        [InlineData("1.0.0-preview2-003121", "https://dotnetcli.blob.core.windows.net/dotnet/preview/Binaries/1.0.0-preview2-003121/dotnet-dev-osx-x64.1.0.0-preview2-003121.tar.gz")]
        [InlineData("1.0.0-preview2-003131", "https://dotnetcli.blob.core.windows.net/dotnet/preview/Binaries/1.0.0-preview2-003131/dotnet-dev-osx-x64.1.0.0-preview2-003131.tar.gz")]
        [InlineData("1.0.0-preview2-003156", "https://dotnetcli.blob.core.windows.net/dotnet/preview/Binaries/1.0.0-preview2-003156/dotnet-dev-osx-x64.1.0.0-preview2-003156.tar.gz")]
        [InlineData("1.0.0-preview2-1-003177", "https://dotnetcli.blob.core.windows.net/dotnet/preview/Binaries/1.0.0-preview2-1-003177/dotnet-dev-osx-x64.1.0.0-preview2-1-003177.tar.gz")]
        [InlineData("1.0.0-preview3-004056", "https://dotnetcli.blob.core.windows.net/dotnet/Sdk/1.0.0-preview3-004056/dotnet-dev-osx-x64.1.0.0-preview3-004056.tar.gz")]
        [InlineData("1.0.0-preview4-004233", "https://dotnetcli.blob.core.windows.net/dotnet/Sdk/1.0.0-preview4-004233/dotnet-dev-osx-x64.1.0.0-preview4-004233.tar.gz")]
        public void CreatesCliDownloadUrl(string version, string expectedUrl)
        {
            new StableAssetChannel()
                .GetDownloadUrl("Microsoft.DotNet.Cli.osx-x64", version)
                .Should()
                .Be(expectedUrl);
        }

        [Theory]
        [InlineData("Microsoft.NETCore.App", "1.1.0")]
        [InlineData("Microsoft.DotNet.Cli.osx-x64", "1.0.0-rc4-004767")]
        public void GetsLatestVersion(string assetId, string version)
        {
            new StableAssetChannel()
                .GetLatestVersion(assetId)
                .Should()
                .Be(version);
        }
    }
}