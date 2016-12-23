using DotNet.Yaml.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotNet.Yaml
{
    public class YamlWriter : IDisposable
    {
        private const string KeyEnd = ": ";
        private const string ListStart = "- ";

        private readonly TextWriter _writer;
        private int _indent = 0;
        private const int TabSize = 4;

        public YamlWriter(Stream stream)
            : this(stream, leaveOpen: false)
        { }

        public YamlWriter(Stream stream, bool leaveOpen)
            :this(new StreamWriter(stream, Encoding.UTF8, bufferSize: 2048, leaveOpen: leaveOpen))
        {
        }

        public YamlWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public void Write(YamlDocument document) { }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}
