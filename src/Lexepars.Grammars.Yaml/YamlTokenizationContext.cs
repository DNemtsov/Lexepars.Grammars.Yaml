using Lexepars.OffsideRule;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lexepars.Grammars.Yaml
{
    public enum DocumentState
    {
        DirectivePreamble,
        Body,
        End
    }

    public class YamlTokenizationContext : OffsideRuleTokenizationContext
    {
        public BlockScalarState BlockScalarState { get; private set; }

        public int IndentationLimit { get; private set; }

        public static ResetDocumentIndent ResetDocumentIndent { get; } = new ResetDocumentIndent();

        public YamlTokenizationContext(LinedInputText text, IEnumerable<FlowExtent> flowExtents)
            : base(text, flowExtents)
        {
        }

        public DocumentState DocumentState { get; private set; }

        protected bool LastIndentWasNotNewLine { get; private set; }

        public override void OnLineTokenizingBegin()
        {
            base.OnLineTokenizingBegin();
            LastIndentWasNotNewLine = true;
        }

        public override void OnProcessIndent(Indent indent, Token token)
        {
            base.OnProcessIndent(indent, token);

            DocumentState = indent == ResetDocumentIndent ? DocumentState.End : DocumentState.Body;

            LastIndentWasNotNewLine = token.Kind != YamlLexer.NewLine;
        }

        public override bool StopIndentLexing()
        {
           if (BlockScalarState == BlockScalarState.None)
                return false;

            return CurrentIndentLevel >= IndentationLimit;
        }

        public override void OnIndentLexingComplete()
        {
            switch (BlockScalarState)
            {
                case BlockScalarState.None:
                    return;
                case BlockScalarState.AwaitingFirstLine:
                    BlockScalarState = BlockScalarState.Within;
                    return;
                case BlockScalarState.AwaitingFirstLineToSetIndent:
                    BlockScalarState = BlockScalarState.Within;
                    IndentationLimit = CurrentIndentLevel;
                    return;
                case BlockScalarState.Within:
                    if (CurrentIndentLevel < IndentationLimit && LastIndentWasNotNewLine)
                        BlockScalarState = BlockScalarState.None;
                    return;
            }
        }

        public override IEnumerable<ScopeState> BalanceScope()
        {
            switch (DocumentState)
            {
                case DocumentState.DirectivePreamble:
                    return Enumerable.Empty<ScopeState>();
                case DocumentState.End:
                    DocumentState = DocumentState.DirectivePreamble;
                    return Enumerable.Repeat(ScopeState.End, ResetIndentation());
            }
            return base.BalanceScope();
        }

        public override Flow.State OnProcessTheFlow(TokenKind tokenKind)
        {
            var flowState = base.OnProcessTheFlow(tokenKind);

            if (flowState != Flow.State.NoExtent)
                DocumentState = DocumentState.Body;

            return flowState;
        }

        protected Regex BlockScalarExplicitIndentRegex { get; } = new Regex(@"\d+");

        public override void OnProcessToken(Token token)
        {
            if (token == null)
                return;

            switch (token.Kind)
            {
                case var t when
                    t == YamlLexer.TagDirective ||
                    t == YamlLexer.VersionDirective ||
                    t == YamlLexer.Comment ||
                    t == YamlLexer.NewLine ||
                    t == YamlLexer.DirectivesEndMarker:
                    break;
                case var m when m == YamlLexer.DocumentEndMarker:
                    DocumentState = DocumentState.End;
                    break;
                default:
                    DocumentState = DocumentState.Body;
                    break;
            }

            if (token.Kind != YamlLexer.BlockScalarHeader)
                return;

            var lexeme = token.Lexeme;

            if (lexeme.Length > 1)
            {
                var match = BlockScalarExplicitIndentRegex.Match(lexeme, 1);

                if (match.Success)
                {
                    var blockLiteralIndent = int.Parse(match.Value, NumberStyles.Integer, CultureInfo.InvariantCulture);

                    IndentationLimit = CurrentIndentLevel + blockLiteralIndent;

                    BlockScalarState = BlockScalarState.AwaitingFirstLine;
                }
                else
                    BlockScalarState = BlockScalarState.AwaitingFirstLineToSetIndent;
            }
        }
    }
}
