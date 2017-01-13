using DotNet.Files;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DotNet.Tests
{
    public class ToolManifestTest
    {
        [Fact]
        public void NetCoreTool()
        {
            var manifest = Deserialize(@"{
    ""version"": 1,
    ""commands"": {
        ""watch"": {
            ""portable"": true,
            ""exe"": ""tools/netcoreapp1.0/dotnet-watch.dll"",
            ""runtimeconfig"": ""tools/netcoreapp1.0/dotnet-watch.runtimeconfig.json""
        }
    }
}");
            manifest.Version.Should().Be(1);
            var value = manifest.Commands.Should()
                .HaveCount(1).And
                .ContainKey("watch").WhichValue;
            value.Exe.Should()
                .Be("tools/netcoreapp1.0/dotnet-watch.dll");
            value.RuntimeConfig.Should()
                .Be("tools/netcoreapp1.0/dotnet-watch.runtimeconfig.json");
            value.Portable.Should().BeTrue();
        }

        private ToolManifest Deserialize(string input)
        {
            return JsonConvert.DeserializeObject<ToolManifest>(input);
        }
    }
}