using System.Globalization;

namespace Lexepars.Tests.IntegrationTests.Yaml.Entities
{
    public class YNumber : YScalar
    {
        public decimal Value { get; }

        public YNumber(decimal value)
        {
            Value = value;
        }

        public static YNumber FromString(string str)
        {
            return new YNumber(decimal.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture));
        }

        public override bool Equals(object obj)
        {
            if (obj is YNumber number)
                return Value == number.Value;

            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
