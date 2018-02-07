using System;
using System.IO;
using DotNet.VersionManager.Files;
using FluentAssertions;
using Xunit;

namespace DotNet.VersionManager.Tests
{
    public class BadConfigFileReaderTest
    {
        [Theory]
        [InlineData("envName:\n  subkey: 1")]
        [InlineData("envName:\n  - item\n  - item2")]
        public void EnvIsNotScalar(string doc)
        {
            Read(doc)
                .ShouldThrow<FormatException>()
                .WithMessage(ConfigFileErrors.EnvNameIsNotScalar);
        }

        [Theory]
        [InlineData("sdk:\n  subkey: 1")]
        [InlineData("sdk:\n  - item\n  - item2")]
        public void CliIsNotScalar(string doc)
        {
            Read(doc)
                .ShouldThrow<FormatException>()
                .WithMessage(ConfigFileErrors.SdkIsNotScalar);
        }

        [Fact]
        public void RuntimeIsAMap()
        {
            Read(@"---
envName: value
runtimes:
  sub: item")
                .ShouldThrow<FormatException>()
                .WithMessage(ConfigFileErrors.RuntimeMustBeListOrScalar);
        }

        [Fact]
        public void RuntimeItemIsAMap()
        {
            Read(@"---
envName: value
runtimes:
   - sub: item")
                .ShouldThrow<FormatException>()
                .WithMessage(ConfigFileErrors.RuntimeSequenceItemIsNotScalar);
        }

        [Fact]
        public void MissingMultipleDoc()
        {
            Read(@"---
envName: value
---
envName: value2")
                .ShouldThrow<FormatException>()
                .WithMessage(ConfigFileErrors.MultipleDocuments);
        }

        [Fact]
        public void ToolIsSequence()
        {
            Read(@"tools:
- name: watch
  version: 1.0.0
- watch: 1.0.0")
                .ShouldThrow<FormatException>()
                .WithMessage(ConfigFileErrors.ToolsMustBeMap);
        }

         [Fact]
        public void ToolItemIsNotScalar()
        {
            Read(@"
tools:
  watch:
    version: 1.0.0")
                .ShouldThrow<FormatException>()
                .WithMessage(ConfigFileErrors.ToolItemsMustBeScalar);
        }

        private Action Read(string doc)
            => () =>
            {
                var reader = new StringReader(doc);
                new ConfigFileYamlReader().Read(reader);
            };
    }
}
