using Lexepars.OffsideRule;

namespace Lexepars.Grammars.Yaml
{
    public class ResetDocumentIndent : Indent
    {
        public ResetDocumentIndent()
            : base(YamlLexer.DocumentEndMarker, ScopePerToken.EmitBefore)
        {
        }

        public override int CalculateNewIndentLevel(int oldIndentLevel, Token token) => 0;
    }
}
