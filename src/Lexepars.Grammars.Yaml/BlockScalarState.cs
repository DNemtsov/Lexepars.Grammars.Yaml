namespace Lexepars.Grammars.Yaml
{
    public enum BlockScalarState
    {
        None,
        AwaitingFirstLine,
        AwaitingFirstLineToSetIndent,
        Within,
    }
}
