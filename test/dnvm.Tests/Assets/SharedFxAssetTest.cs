using DotNet.Assets;
using Xunit;

namespace DotNet.Test
{
    public class SharedFxAssetTest
    {
        [Theory]
        [InlineData("1.0.1", "https://dotnetcli.blob.core.windows.net/dotnet/preview/Binaries/1.0.1/dotnet-osx-x64.1.0.1.tar.gz")]
        [InlineData("1.1.0-preview1-001100-00", "https://dotnetcli.blob.core.windows.net/dotnet/release/1.1.0/Binaries/1.1.0-preview1-001100-00/dotnet-osx-x64.1.1.0-preview1-001100-00.tar.gz")]
        public void CreatesDownloadUrl(string version, string expectedUrl)
        {
            Assert.Equal(expectedUrl, SharedFxAsset.CreateDownloadUrl(version));
        }
    }
}