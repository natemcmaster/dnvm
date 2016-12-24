using System;
using System.IO;
using System.Threading.Tasks;
using DotNet.Assets;
using DotNet.Commands;
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

        [Fact]
        public async Task GivenThatIWantToInstallFx_1_0_0()
        {
            var command = new InstallCommand(SharedFxAsset.Name, "1.0.0");
            var context = new CommandContext
            {
                Reporter = new TestReporter(_output),
                Environment = new Files.Environment("test", new DirectoryInfo(_tempDir.Path))
            };

            await command.ExecuteAsync(context).OrTimeout(90);

            Directory.EnumerateFiles(_tempDir.Path)
                .Should().Contain(f => Path.GetFileName(f) == "dotnet");

            Directory.Exists(Path.Combine(_tempDir.Path, "shared", "Microsoft.NETCore.App", "1.0.0")).Should().BeTrue();
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