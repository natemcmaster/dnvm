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

namespace DotNet.VersionManager.Test
{
    public class SharedRuntimeInstallTest : IDisposable
    {
        private readonly TempDir _tempDir = new TempDir();
        private readonly ITestOutputHelper _output;

        [Theory]
        // commented out to improve test time. Each test downloads ~50MB
        // [InlineData("1.0.0")]
        // [InlineData("1.0.1")]
        [InlineData("1.0.3")]
        [InlineData("1.1.0")]
        public async Task InstallsFx(string version)
        {
            var command = new InstallRuntimeCommand(version, Architecture.X64);
            var context = new CommandContext
            {
                Logger = new TestLogger(_output),
                Environment = new DotNetEnv("test", new DirectoryInfo(_tempDir.Path))
            };

            await command.ExecuteAsync(context).OrTimeout(90);

            context.Result.Should().Be(Result.Okay);

            Directory.EnumerateFiles(_tempDir.Path)
                .Should().Contain(f => Path.GetFileName(f) == "dotnet");

            Directory.Exists(Path.Combine(_tempDir.Path, "shared", "Microsoft.NETCore.App", version)).Should().BeTrue();
        }

        public SharedFxInstallTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public void Dispose()
        {
            _tempDir.Dispose();
        }
    }
}
