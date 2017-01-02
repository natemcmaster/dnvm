using System;
using System.IO;
using System.Threading.Tasks;
using DotNet.Assets;
using DotNet.Commands;
using DotNet.Files;
using DotNet.Utils;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Test
{
    public class SharedFxInstallTest : IDisposable
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
            var command = new InstallCommand<SharedFxAsset>(version);
            var context = new CommandContext
            {
                Reporter = new TestReporter(_output),
                Environment = new DotNetEnv("test", new DirectoryInfo(_tempDir.Path))
            };

            await command.ExecuteAsync(context).OrTimeout(90);

            context.Result.Should().Be(Result.Done);

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