namespace Lexepars.Tests.IntegrationTests.Yaml.Entities
{
    public class YBoolean : YScalar
    {
        public bool Value { get; }

        public YBoolean(bool value)
        {
            Value = value;
        }

        public static readonly YBoolean True = new YBoolean(true);
        public static readonly YBoolean False = new YBoolean(false);

        public override bool Equals(object obj)
        {
            if (obj is YBoolean boo)
                return Value == boo.Value;

            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
