using System;
using System.IO;
using System.Threading.Tasks;
using DotNet.Assets;
using DotNet.Commands;
using DotNet.Files;
using DotNet.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Test
{
    public class CliInstallTest : IDisposable
    {
        private readonly TempDir _tempDir = new TempDir();
        private readonly ITestOutputHelper _output;

        [Theory]
        // commented out to improve test time. Each test downloads ~50MB
        // [InlineData("1.0.0-preview2-003121", "1.0.0")]
        // [InlineData("1.0.0-preview2-003131", "1.0.1")]
        // [InlineData("1.0.0-preview2-003156", "1.0.3")]
        // [InlineData("1.0.0-preview2-1-003177", "1.1.0")]
        // [InlineData("1.0.0-preview3-004056", "1.0.1")]
        [InlineData("1.0.0-preview4-004233", "1.0.1")]
        public async Task InstallsCliWithSharedFx(string version, string fxVersion)
        {
            var command = new InstallCommand<SdkAsset>(version);
            var context = new CommandContext
            {
                Reporter = new TestReporter(_output),
                Environment = new DotNetEnv("test", new DirectoryInfo(_tempDir.Path)),
                Services = CreateTestServices()
            };

            await command.ExecuteAsync(context).OrTimeout(90);

            context.Result.Should().Be(Result.Done);

            Directory.EnumerateFiles(_tempDir.Path)
                .Should().Contain(f => Path.GetFileName(f) == "dotnet");

            Directory.Exists(Path.Combine(_tempDir.Path, "shared", "Microsoft.NETCore.App", fxVersion)).Should().BeTrue();
            Directory.Exists(Path.Combine(_tempDir.Path, "sdk", version)).Should().BeTrue();
        }

        public CliInstallTest(ITestOutputHelper output)
        {
            _output = output;
        }

        private IServiceProvider CreateTestServices()
        {
            return new ServiceCollection()
                .AddDnvm()
                .BuildServiceProvider();
        }

        public void Dispose()
        {
            _tempDir.Dispose();
        }
    }
}