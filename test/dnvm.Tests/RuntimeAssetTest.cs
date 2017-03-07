using System.Runtime.InteropServices;
using DotNet.VersionManager.Assets;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DotNet.VersionManager.Tests
{
    public class RuntimeAssetTest
    {
        [Theory]
        [InlineData("1.0.0")]
        [InlineData("1.0.1")]
        [InlineData("1.0.3")]
        [InlineData("1.0.4")]
        [InlineData("1.1.0")]
        [InlineData("1.1.1")]
        public void HasOpenSSLDependency(string version)
        {
            var asset = new RuntimeAsset(NullLogger.Instance,
                TestDotNetEnv.Create(),
                version,
                Architecture.X64);
            asset.Dependencies
                .Should()
                .AllBeOfType<OpenSslAsset>()
                .And.HaveCount(1);
        }
    }
}
