namespace Lexepars.Grammars.Yaml.Entities
{
    public class YNull : YScalar
    {
        private YNull()
        {
        }

        public static readonly YNull Null = new YNull();

        public override bool Equals(object obj)
        {
            return obj is YNull;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
