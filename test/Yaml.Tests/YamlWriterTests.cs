using DotNet.Yaml.Model;
using System;
using System.IO;
using Xunit;

namespace DotNet.Yaml.Tests
{
    public class YamlWriterTests : IDisposable
    {
        private Stream _stream = new MemoryStream();

        [Fact]
        public void WriteNullDoc()
        {
            var doc = new YamlDocument();

            Write(doc);
            AssertDoc(@"");
        }

        private void AssertDoc(string expected)
        {
            _stream.Seek(0, SeekOrigin.Begin);
            string actual;
            using (var reader = new StreamReader(_stream))
            {
                actual = reader.ReadToEnd();
            }
            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }

        private void Write(YamlDocument doc)
        {
            using (var writer = new YamlWriter(_stream, leaveOpen: true))
            {
                writer.Write(doc);
            }
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
