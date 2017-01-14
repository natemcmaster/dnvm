using System.IO;
using System.Text;
using DotNet.Files;
using FluentAssertions;
using Xunit;

namespace DotNet.Test
{
    public class ConfigFileYamlWriterTest
    {
        [Fact]
        public void Composite()
        {
            Write(new ConfigFile
            {
                Sdk = "1.0.0-preview4",
                SharedFx = { "1.0.1", "1.1.0" },
                Environment = "default",
                Tools =
                {
                    ["prop"] = "1.1.0"
                }
            })
           .Should()
           .Be("env: default\nsdk: 1.0.0-preview4\nfx:\n  - 1.0.1\n  - 1.1.0\ntools:\n  prop: 1.1.0\n");
        }

        [Fact]
        public void Empty()
        {
            Write(new ConfigFile()).Should().BeEmpty();
        }

        [Fact]
        public void Env()
        {
            Write(new ConfigFile
            {
                Environment = "default"
            })
            .Should()
            .Be("env: default\n");
        }

        [Fact]
        public void Sdk()
        {
            Write(new ConfigFile
            {
                Sdk = "1.0.0-preview4"
            })
            .Should()
            .Be("sdk: 1.0.0-preview4\n");
        }

        [Fact]
        public void OneFx()
        {
            Write(new ConfigFile
            {
                SharedFx = { "1.0.1" }
            })
            .Should()
            .Be("fx: 1.0.1\n");
        }

        [Fact]
        public void MultipleFx()
        {
            Write(new ConfigFile
            {
                SharedFx = { "stable", "1.0.0" }
            })
            .Should()
            .Be("fx:\n  - stable\n  - 1.0.0\n");
        }


        private string Write(ConfigFile file)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            new ConfigFileYamlWriter().Write(writer, file);
            return sb.ToString();
        }
    }
}