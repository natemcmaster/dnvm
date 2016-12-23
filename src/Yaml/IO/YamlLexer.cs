using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotNet.Yaml.IO
{
    public class YamlLexer
    {
        private readonly TextReader _reader;

        public YamlLexer(TextReader reader)
        {
            _reader = reader;
        }

        private int _line = 1;
        private int _col = 0;

        public IEnumerable<YamlToken> Tokenize()
        {
            yield return YamlToken.DocumentBegin;
            int next;
            while ((next = _reader.Peek()) >= 0)
            {
                yield return ProduceToken((char)next);
            }
            yield return YamlToken.DocumentEnd;
        }

        private YamlToken ProduceToken(char start)
        {
            switch (start)
            {
                case ' ':
                    {
                        var count = 1;
                        TryRead(out char _); // consume space
                        while (TryPeek(out char next) && next == ' ')
                        {
                            TryRead(out char __); // consume space
                            count++;
                        }
                        return YamlToken.Spaces(count);
                    }
                case '\t':
                    {
                        var count = 1;
                        TryRead(out char _); // consume 
                        while (TryPeek(out char next) && next == '\t')
                        {
                            TryRead(out char __); // consume
                            count++;
                        }
                        return YamlToken.Tabs(count);
                    }
                case '\r':
                    {
                        TryRead(out char _); // consume \r
                        if (!TryPeek(out char next) || next != '\n')
                        {
                            throw ErrorAtCurrentPosition("Invalid line endings sequences");
                        }
                        TryRead(out char __); // consume \n
                        return YamlToken.NewLine;
                    }
                case '\n':
                    {
                        TryRead(out char _); // consume char
                        return YamlToken.NewLine;
                    }
                case ':':
                    {
                        TryRead(out char _); // consume char
                        return YamlToken.SemiColon;
                    }
                case '#':
                    {
                        TryRead(out char _); //consume char
                        var sb = new StringBuilder();
                        while (TryPeek(out char next) && next != '\r' && next != '\n')
                        {
                            sb.Append(next);
                            TryRead(out char __); // consume next
                        }
                        return YamlToken.Comment(sb.ToString());
                    }
                case '\"':
                    {
                        var sb = new StringBuilder();
                        TryRead(out char _); // consume quote
                        while (TryRead(out char next))
                        {
                            switch (next)
                            {
                                case '\"':
                                    TryRead(out char __); // consume quote
                                    return YamlToken.String(sb.ToString());
                                case '\\':
                                    {
                                        // escape character
                                        if (TryRead(out next))
                                        {
                                            switch (next)
                                            {
                                                case 'n':
                                                    sb.Append('\n');
                                                    break;
                                                case 'r':
                                                    sb.Append('\r');
                                                    break;
                                                case 't':
                                                    sb.Append('\t');
                                                    break;
                                                case '"':
                                                    sb.Append(next);
                                                    break;
                                                default:
                                                    throw ErrorAtCurrentPosition($"Unrecognized escape characters '\\{next}'");
                                            }
                                        }
                                        else
                                        {
                                            throw UnexpectedDocumentEnd();
                                        }
                                    }
                                    break;
                                case '\r':
                                case '\n':
                                    throw new FormatException($"Expected closing quote at {_line - 1 } before lined ended");
                                default:
                                    sb.Append(next);
                                    break;
                            }
                        }
                        throw new FormatException("Expected closing quote before the document ended");
                    }
                default:
                    {
                        var sb = new StringBuilder(start);
                        while (TryPeek(out char next) && !char.IsWhiteSpace(next))
                        {
                            sb.Append(next);
                            TryRead(out char _);
                        }
                        return YamlToken.String(sb.ToString());
                    }
            }
        }

        private bool TryPeek(out char next)
        {
            var ch = _reader.Peek();
            if (ch < 0)
            {
                next = '\0';
                return false;
            }

            next = (char)ch;
            return true;
        }

        private bool TryRead(out char next)
        {
            var ch = _reader.Read();
            if (ch < 0)
            {
                next = '\0';
                return false;
            }

            _col++;
            if (ch == '\n')
            {
                _col = 0;
                _line++;
            }

            next = (char)ch;
            return true;
        }

        private Exception UnexpectedDocumentEnd()
        {
            return new FormatException("Unexpected end of document");
        }

        private Exception ErrorAtCurrentPosition(string message)
        {
            return new FormatException($"{message} at line { _line } position { _col}");
        }
    }
}
