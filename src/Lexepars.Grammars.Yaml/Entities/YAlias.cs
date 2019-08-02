using System;

namespace Lexepars.Tests.IntegrationTests.Yaml.Entities
{
    public class YAlias: YNode
    {
        public string Value { get; }

        public YAlias(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override string ToString() => $"YAlias: {Value}";

        public static YAlias FromString(string value) => new YAlias(value.Substring(1));

        public override bool Equals(object obj)
        {
            if (obj is YAlias str)
                return Value == str.Value;

            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}
