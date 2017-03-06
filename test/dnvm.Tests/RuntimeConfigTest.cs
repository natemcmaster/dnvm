using DotNet.VersionManager.Files;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DotNet.VersionManager.Tests
{
    public class RuntimeConfigTest
    {
        [Fact]
        public void ReadsRuntimeConfigFile()
        {
            var runtimeconfig = Deserialize(@"{
  ""runtimeOptions"": {
    ""framework"": {
      ""name"": ""Microsoft.NETCore.App"",
      ""version"": ""1.0.0""
    }
  }
}");

            runtimeconfig.RuntimeOptions?.Framework
                .Should().NotBeNull();
            runtimeconfig.RuntimeOptions.Framework.Name
                .Should().Be("Microsoft.NETCore.App");
            runtimeconfig.RuntimeOptions.Framework.Version
                .Should().Be("1.0.0");
        }

        private RuntimeConfig Deserialize(string input)
        {
            return JsonConvert.DeserializeObject<RuntimeConfig>(input);
        }
    }
}
