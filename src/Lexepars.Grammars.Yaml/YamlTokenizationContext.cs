using Lexepars.OffsideRule;
using System.Collections.Generic;
using System.Globalization;

using System.Text.RegularExpressions;

namespace Lexepars.Tests.IntegrationTests.Yaml
{
    public enum BlockScalarState
    {
        None,
        AwaitingFirstLine,
        AwaitingFirstLineToSetIndent,
        Within,
    }

    public class YamlTokenizationContext : OffsideRuleTokenizationContext
    {
        public BlockScalarState BlockScalarState { get; private set; }

        public int IndentationLimit { get; private set; }

        public YamlTokenizationContext(LinedInputText text, IEnumerable<FlowExtent> flowExtents)
            : base(text, flowExtents)
        {
        }

        protected bool LastIndentWasNotNewLine { get; set; }

        public override void OnLineTokenizingBegin()
        {
            base.OnLineTokenizingBegin();
            LastIndentWasNotNewLine = true;
        }

        public override void OnProcessIndent(Indent indent, Token token)
        {
            base.OnProcessIndent(indent, token);

            LastIndentWasNotNewLine = token.Kind != YamlLexer.NewLine;
        }

        public override bool NoMoreIndents()
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

        protected Regex BlockScalarExplicitIndentRegex { get; } = new Regex(@"\d+");

        public override void OnProcessToken(Token token)
        {
            if (token == null)
                return;

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
