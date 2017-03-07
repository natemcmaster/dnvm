using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DotNet.VersionManager.Commands;
using DotNet.VersionManager.Files;
using DotNet.VersionManager.Utils;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.VersionManager.Tests
{
    public class CliInstallTest : IDisposable
    {
        private readonly TempDir _tempDir = new TempDir();
        private readonly ITestOutputHelper _output;

        [Theory]
        // commented out to improve test time. Each test downloads ~50MB
        // [InlineData("1.0.0-preview2-003121", new [] { "1.0.0" })]
        // [InlineData("1.0.0-preview2-003156", new [] { "1.0.3" })]
        // [InlineData("1.0.0-preview2-1-003177", new [] { "1.1.0" })]
        // [InlineData("1.0.0-rc3-004530", new [] { "1.0.3" })]
        [InlineData("1.0.1", new[] { "1.0.4", "1.1.1" })]
        public async Task InstallsCliWithSharedFx(string version, string[] runtimes)
        {
            var command = new InstallSdkCommand(version, Architecture.X64);
            var context = new CommandContext
            {
                Logger = new TestLogger(_output),
                Environment = new DotNetEnv("test", new DirectoryInfo(_tempDir.Path))
            };

            await command.ExecuteAsync(context).OrTimeout(180);

            context.Result.Should().Be(Result.Okay);

            Directory.EnumerateFiles(_tempDir.Path)
                .Should().Contain(f => Path.GetFileName(f) == "dotnet");

            Directory.Exists(Path.Combine(_tempDir.Path, "sdk", version)).Should().BeTrue();

            foreach (var runtime in runtimes)
            {
                Directory.Exists(Path.Combine(_tempDir.Path, "shared", "Microsoft.NETCore.App", runtime)).Should().BeTrue();
            }
        }

        public CliInstallTest(ITestOutputHelper output)
        {
            _output = output;
        }
        public void Dispose()
        {
            _tempDir.Dispose();
        }
    }
}
