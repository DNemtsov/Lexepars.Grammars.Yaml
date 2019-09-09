using System;
using System.Text.RegularExpressions;

namespace Lexepars.Grammars.Yaml.Entities
{
    public class YVersionDirective : YDirective
    {
        public string Version { get; }

        public YVersionDirective(string version)
        {
            Version = version;
        }

        public static YVersionDirective FromLexeme(string directiveLexeme)
        {
            var regex = new Regex(@"%YAML\s(\d\.\d+)");

            var match = regex.Match(directiveLexeme);

            if (!match.Success)
                throw new ArgumentException("invalid language version directive lexeme.", nameof(directiveLexeme));

            var version = match.Groups[1].Value;

            return new YVersionDirective(version);
        }

        public override bool Equals(object obj)
        {
            if (obj is YVersionDirective version)
                return Version == version.Version;

            return false;
        }

        public override int GetHashCode() => Version.GetHashCode();
    }
}
