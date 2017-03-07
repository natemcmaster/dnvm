using System.Linq;
using System.Runtime.InteropServices;
using DotNet.VersionManager.Assets;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DotNet.VersionManager.Tests
{
    public class SdkAssetTest
    {
        [Fact]
        public void ListsRuntimeAssetsAsDependencies()
        {
            var asset = new SdkAsset(NullLogger.Instance,
                TestDotNetEnv.Create(),
                "1.0.1",
                Architecture.X64);
            asset.Dependencies
                .Should()
                .AllBeOfType<RuntimeAsset>()
                .And.HaveCount(2);

            asset.Dependencies
                .Cast<RuntimeAsset>()
                .Select(r => r.Version)
                .Should()
                .Contain(new[] { "1.0.4", "1.1.1" });
        }
    }
}
