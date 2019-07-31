namespace Lexepars.Tests.IntegrationTests.Yaml
{
    using Lexepars.OffsideRule;
    using System;
    using System.Collections.Generic;

    public class YamlLexer : OffsideRuleLexer<YamlTokenizationContext>
    {
        public YamlLexer()
            : base(
                null,
                new[]
                {
                    Indent.LexemeLength(Space),
                    Indent.LexemeLength(BlockSequenceItemMarker, ScopePerToken.EmitBefore),
                    Indent.Ignore(NewLine)
                },
                new[]
                {
                    new FlowExtent(FlowSequenceBegin, FlowSequenceEnd),
                    new FlowExtent(FlowMappingBegin, FlowMappingEnd),
                },        
                Skip(Space),
                Skip(NewLine),
                BlockSequenceItemMarker,
                Null,
                True,
                False,
                FlowItemSeparator,
                FlowSequenceBegin,
                FlowSequenceEnd,
                FlowMappingBegin,
                FlowMappingEnd,
                MappingSeparator,
                Number,
                DoubleQuotedString,
                SingleQuotedString,
                BlockScalarHeader,
                Comment,
                Anchor,
                Alias,
                UnquotedString)
        { }

        public static readonly MatchableTokenKind Space = new OperatorTokenKind(" ");

        public static readonly MatchableTokenKind NewLine = new PatternTokenKind("new line", @"(?:\r\n?)|\n");

        public static readonly MatchableTokenKind Null = new KeywordTokenKind("null");

        public static readonly MatchableTokenKind True = new KeywordTokenKind("true");

        public static readonly MatchableTokenKind False = new KeywordTokenKind("false");

        public static readonly MatchableTokenKind BlockSequenceItemMarker = new PatternTokenKind("seq entry marker", "(-[ ])|(-(?=[\r\n]))");

        public static readonly MatchableTokenKind FlowItemSeparator = new OperatorTokenKind(",");

        public static readonly MatchableTokenKind FlowSequenceBegin = new OperatorTokenKind("[");

        public static readonly MatchableTokenKind FlowSequenceEnd = new OperatorTokenKind("]");

        public static readonly MatchableTokenKind FlowMappingBegin = new OperatorTokenKind("{");

        public static readonly MatchableTokenKind FlowMappingEnd = new OperatorTokenKind("}");

        public static readonly MatchableTokenKind MappingSeparator = new OperatorTokenKind(":");

        public static readonly MatchableTokenKind DoubleQuotedString = new PatternTokenKind("double-quoted string", @"
            # Open quote:
            ""

            # Zero or more content characters:
            (
                      [^""\\]*             # Zero or more non-quote, non-slash characters.
                |     \\ [""\\bfnrt\/]     # One of: slash-quote   \\   \b   \f   \n   \r   \t   \/
                |     \\ u [0-9a-fA-F]{4}  # \u folowed by four hex digits
            )*

            # Close quote:
            ""
        ");

        public static readonly MatchableTokenKind SingleQuotedString = new PatternTokenKind("single-quoted string", "'[^']*'");

        public static readonly MatchableTokenKind UnquotedString = new PatternTokenKind("unquoted string", @"
            # One or more of the sharacters not participating in the syntax
            [^""\\\[\]\{\},\:\s][^""\\\[\]\{\},\:\r\n]*
        ");

        public static readonly MatchableTokenKind Number = new PatternTokenKind("number", @"([+-]?)(?=\d|\.\d)\d*(\.\d*)?([Ee]([+-]?\d+))?");

        public static readonly MatchableTokenKind BlockScalarHeader = new PatternTokenKind("block scalar header", @"[\|>](?:(?:\d*[+-]?)|(?:[+-]?\d*))(?=[\s\r\n\#]+)");

        public static readonly MatchableTokenKind Comment = new PatternTokenKind("comment", @"\#[^\r\n]*");

        public static readonly MatchableTokenKind BlockScalarLine = new PatternTokenKind("block scalar line", @"[^\r\n]*(?:\r\n?|\n)?");

        public static readonly MatchableTokenKind Anchor = new PatternTokenKind("anchor", @"&[^\r\n\[\]\{\},\s]+");

        public static readonly MatchableTokenKind Alias = new PatternTokenKind("alias", @"\*[^\r\n\[\]\{\},\s]+");

        protected override sealed YamlTokenizationContext CreateTokenizationContext(LinedInputText text, IEnumerable<FlowExtent> flowExtents)
        {
            return new YamlTokenizationContext(text, flowExtents);
        }

        protected override sealed Token GetToken(ILinedInputText text, YamlTokenizationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.BlockScalarState != BlockScalarState.Within)
                return base.GetToken(text, context);

            BlockScalarLine.TryMatch(text, out Token token);

            if (token != null)
                return token;

            NewLine.TryMatch(text, out Token newLine);

            return newLine;
        }

        protected override bool ShouldSkipToken(Token token, YamlTokenizationContext context)
        {
            if (context.BlockScalarState == BlockScalarState.Within)
            {
                if (token.Kind == NewLine)
                    return false;
            }

            return base.ShouldSkipToken(token, context);
        }
    }
}