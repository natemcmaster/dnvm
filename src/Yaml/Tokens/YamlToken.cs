using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DotNet.Yaml
{
    public enum TokenKind
    {
        // no length
        DocumentBegin,
        DocumentEnd,

        // single char
        SemiColon,
        Comment,
        DoubleQuote,

        // var length
        Tabs,
        String,
        Spaces,
        NewLine
    }

    [DebuggerDisplay("Token({Kind}, {Value})")]
    public class YamlToken
    {
        public static readonly YamlToken SemiColon = new YamlToken(TokenKind.SemiColon, ":");
        public static readonly YamlToken DoubleQuote = new YamlToken(TokenKind.DoubleQuote, "\"");
        public static readonly YamlToken DocumentBegin = new YamlToken(TokenKind.DocumentBegin, null);
        public static readonly YamlToken DocumentEnd = new YamlToken(TokenKind.DocumentEnd, null);
        public static readonly YamlToken NewLine = new YamlToken(TokenKind.NewLine, null);

        public static YamlToken String(string value)
            => new YamlToken(TokenKind.String, value);

        public static YamlToken Comment(string comment)
            => new YamlToken(TokenKind.Comment, comment);

        public static YamlToken Spaces(int count)
            => new YamlToken(TokenKind.Spaces, new string(' ', count));

        public static YamlToken Tabs(int count)
            => new YamlToken(TokenKind.Tabs, new string('\t', count));

        private YamlToken(TokenKind kind, string value)
        {
            Kind = kind;
            Value = value ?? string.Empty;
        }

        public TokenKind Kind { get; }

        public string Value { get; }

        public override string ToString()
        {
            return $"Token({Kind}, {Value})";
        }

        public override int GetHashCode()
        {
            int code;
            unchecked
            {
                code = Kind.GetHashCode();
                code = code * 37 + (Value?.GetHashCode() ?? 0);
            }
            return code;
        }

        public override bool Equals(object obj)
        {
            return Equals(this, obj);
        }

        public static bool Equals(YamlToken left, object obj)
        {
            if (ReferenceEquals(left, obj))
            {
                return true;
            }
            if (left == null || obj == null)
            {
                return false;
            }
            if (obj is YamlToken right)
            {
                return left.Kind == right.Kind && left.Value == right.Value;
            }
            return false;
        }
    }
}
