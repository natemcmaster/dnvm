using System.IO;
using DotNet.Files;
using FluentAssertions;
using Xunit;

namespace DotNet.Test
{
    public class ConfigFileReaderTest
    {
        [Fact]
        public void MultipleFx()
        {
            var file = Read(@"
env: myenvname
fx:
 - 1.0.1
 - 1.2.3
 - alpha
sdk: 1.0.0-preview3-003131
tools:
  ef: 1.0.0
  watch: stable ");

            file.Should().NotBeNull();
            file.Environment.Should().Equals("myenvname");
            file.SharedFx.Should()
                .HaveCount(3)
                .And.Contain(new[] { "1.0.1", "1.2.3", "alpha" });
            file.Sdk.Should().Equals("1.0.0-preview3-003131");
            file.Tools.Should()
                .HaveCount(2)
                .And.ContainKey("ef").WhichValue.Should().Be("1.0.0");
            file.Tools.Should().ContainKey("watch").WhichValue.Should().Be("stable");
        }

        [Fact]
        public void SingleFx()
        {
            var file = Read(@"
env: myenvname
fx: 1.1.0");

            file.Should().NotBeNull();
            file.Environment.Should().Equals("myenvname");
            file.SharedFx.Should().ContainSingle("1.1.0");
        }

        [Fact]
        public void Comments()
        {
            var file = Read(@"
# comments
env: myenvname
# more comments

fx: 1.1.0 # trailing comments

#end comments");

            file.Should().NotBeNull();
            file.Environment.Should().Equals("myenvname");
            file.SharedFx.Should().ContainSingle("1.1.0");
        }


        private ConfigFile Read(string doc)
        {
            var reader = new StringReader(doc);
            return new ConfigFileYamlReader().Read(reader);
        }
    }
}