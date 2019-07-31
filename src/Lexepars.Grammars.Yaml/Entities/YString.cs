using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexepars.Tests.IntegrationTests.Yaml.Entities
{
    public class YString : YScalar
    {
        public string Value { get; }

        public YString(string value)
        {
            Value = value;
        }

        public static YString FromString(string str)
        {
            var result = 
                Regex.Replace(
                    str,
                    @"\\u[0-9a-fA-F]{4}",
                    match => char.ConvertFromUtf32(
                        int.Parse(match.Value.Replace("\\u", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture)));

            result = result
                .Replace("\\\"", "\"")
                .Replace("\\\\", "\\")
                .Replace("\\b", "\b")
                .Replace("\\f", "\f")
                .Replace("\\n", "\n")
                .Replace("\\r", "\r")
                .Replace("\\t", "\t")
                .Replace("\\/", "/");

            return new YString(str);
        }

        public static YString FromQuotedString(string str)
        {
            var strippedOfQuotes = str.Substring(1, str.Length - 2);

            return FromString(strippedOfQuotes);
        }

        public static YString FromBlockScalar(string header, IList<string> lines)
        {
            if (header == null)
                throw new ArgumentNullException(nameof(header));

            if (lines == null)
                throw new ArgumentNullException(nameof(lines));

            var folded = header[0] == '>';

            var sb = new StringBuilder();

            var chomping = GetChomping(header);

            var lastMeaningfulLineIndex = lines.Count - 1;

            bool NonemptyLine(string line) => line.Any(ch => !char.IsWhiteSpace(ch));

            if (chomping != BlockScalarChomping.Keep)
            {
                for (int lineIndex = lines.Count - 1; lineIndex >= 0; --lineIndex)
                {
                    if (NonemptyLine(lines[lineIndex]))
                        break;

                    lastMeaningfulLineIndex = lineIndex - 1;
                }
            }

            var includeLineBody = !folded;

            for (int lineIndex = 0; lineIndex <= lastMeaningfulLineIndex; ++lineIndex)
            {
                var line = lines[lineIndex];

                var moreIndentedLine = folded && char.IsWhiteSpace(line[0]);

                var includePreviousLineBody = includeLineBody;
                includeLineBody = !folded || NonemptyLine(line);

                if (lineIndex > 0)
                {
                    if (folded && !moreIndentedLine)
                    {
                        if (includePreviousLineBody && includeLineBody)
                            sb.Append(' ');
                    }
                    else
                        sb.Append('\n');
                }

                for (var charIndex = 0; charIndex < line.Length; ++charIndex)
                {
                    var ch = line[charIndex];

                    if (ch == '\r' || ch == '\n')
                    {
                        var lastLine = lineIndex == lastMeaningfulLineIndex;

                        if (lastLine && chomping != BlockScalarChomping.Strip)
                            sb.Append('\n');

                        break;
                    }

                    if (includeLineBody)
                        sb.Append(ch);
                }
            }

            return new YString(sb.ToString());
        }

        protected enum BlockScalarChomping
        {
            Keep,
            Clip,
            Strip
        }

        protected static BlockScalarChomping GetChomping(string header)
        {
            var match = new Regex(@"[+-]", RegexOptions.Compiled).Match(header, 1);

            if (!match.Success)
                return BlockScalarChomping.Clip;

            return match.Value == "+" ? BlockScalarChomping.Keep : BlockScalarChomping.Strip;
        }

        public override bool Equals(object obj)
        {
            if (obj is YString str)
               return Value == str.Value;

            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"YString: {Value}";
    }
}
