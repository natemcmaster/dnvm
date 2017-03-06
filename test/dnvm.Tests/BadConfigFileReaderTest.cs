using System;
using System.IO;
using DotNet.VersionManager.Files;
using FluentAssertions;
using Xunit;

namespace DotNet.VersionManager.Test
{
    public class BadConfigFileReaderTest
    {
        [Fact]
        public void MissingEnvKey()
        {
            Read(@"")
                .ShouldThrow<FormatException>()
                .WithMessage(ConfigFileErrors.MissingEnvKey);
        }

        [Theory]
        [InlineData("env:\n  subkey: 1")]
        [InlineData("env:\n  - item\n  - item2")]
        public void EnvIsNotScalar(string doc)
        {
            Read(doc)
                .ShouldThrow<FormatException>()
                .WithMessage(ConfigFileErrors.EnvIsNotScalar);
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
        public void FxIsAMap()
        {
            Read(@"---
env: value
fx:
  sub: item")
                .ShouldThrow<FormatException>()
                .WithMessage(ConfigFileErrors.FxMustBeListOrScalar);
        }

        [Fact]
        public void FxItemIsAMap()
        {
            Read(@"---
env: value
fx:
   - sub: item")
                .ShouldThrow<FormatException>()
                .WithMessage(ConfigFileErrors.FxSequenceItemIsNotScalar);
        }

        [Fact]
        public void MissingMultipleDoc()
        {
            Read(@"---
env: value
---
env: value2")
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
