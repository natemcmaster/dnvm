using DotNet.Yaml.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace DotNet.Yaml.Tests
{
    public class LexerTests
    {
        [Fact]
        public void Dictionary()
        {
            var tokens = Tokenize(@"key: value1
""quoted key"": value2
key2:value:withcolon
");
            AssertTokens(new[]
            {
                YamlToken.DocumentBegin,
                YamlToken.String("key"),
                YamlToken.SemiColon,
                YamlToken.Spaces(1),
                YamlToken.String("value1"),
                YamlToken.NewLine,
                YamlToken.String("quoted key"),
                YamlToken.SemiColon,
                YamlToken.Spaces(1),
                YamlToken.String("value2"),
                YamlToken.NewLine,
                YamlToken.String("key2"),
                YamlToken.SemiColon,
                YamlToken.String("value:withcolon"),
                YamlToken.NewLine,
                YamlToken.DocumentEnd,
            }, tokens);
        }

        [Fact]
        public void Comments()
        {
            var tokens = Tokenize(
@"# comment at beginning
");
            AssertTokens(new[]
            {
                YamlToken.DocumentBegin,
                YamlToken.Comment(" comment at beginning"),
                YamlToken.NewLine,
                YamlToken.DocumentEnd
            },
            tokens);
        }

        private IEnumerable<YamlToken> Tokenize(string doc)
        {
            var reader = new StringReader(doc);
            return new YamlLexer(reader).Tokenize();
        }

        private void AssertTokens(IEnumerable<YamlToken> expected, IEnumerable<YamlToken> actual)
        {
            var left = expected.GetEnumerator();
            var right = actual.GetEnumerator();

            var leftCount = 0;
            while (left.MoveNext())
            {
                leftCount++;
                if (!right.MoveNext())
                {
                    throw new XunitException($"Unexpected end to sequence of tokens. Expected {left.Current}");
                }
                if (!left.Current.Equals(right.Current))
                {
                    throw new XunitException($"Unexpected token at position {leftCount}. Expected {left.Current} but got {right.Current}");
                }
            }
            if (right.MoveNext())
            {
                throw new XunitException($"Expected end to sequence of tokens but found {right.Current}");
            }
        }
    }
}
