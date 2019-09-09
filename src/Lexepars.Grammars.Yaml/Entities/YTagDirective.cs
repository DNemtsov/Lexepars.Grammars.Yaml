using System;
using System.Text.RegularExpressions;

namespace Lexepars.Grammars.Yaml.Entities
{
    public class YTagDirective : YDirective
    {
        public string Handle { get; }

        public string Prefix { get; }

        public YTagDirective(string handle, string prefix)
        {
            Handle = handle;
            Prefix = prefix;
        }

        public static YTagDirective FromLexeme(string lexeme)
        {
            var regex = new Regex(@"%TAG\s((?:!\w*!)|!)\s(!?\w.*)");

            var match = regex.Match(lexeme);

            if (!match.Success)
                throw new ArgumentException("invalid tag directive lexeme.", nameof(lexeme));

            var handle = match.Groups[1].Value;
            var prefix = match.Groups[2].Value;

            return new YTagDirective(handle, prefix);
        }

        public override bool Equals(object obj)
        {
            if (obj is YTagDirective tag)
                return Handle == tag.Handle && Prefix == tag.Prefix;

            return false;
        }

        public override int GetHashCode() => Handle.GetHashCode() ^ Prefix.GetHashCode();
    }
}
