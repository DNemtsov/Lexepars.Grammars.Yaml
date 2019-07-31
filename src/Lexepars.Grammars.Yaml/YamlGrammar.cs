using Lexepars.OffsideRule;
using Lexepars.Tests.IntegrationTests.Yaml.Entities;

namespace Lexepars.Tests.IntegrationTests.Yaml
{
    public class YamlGrammar : Grammar<YNode>
    {

        static readonly GrammarRule<YBoolean> Boolean = new GrammarRule<YBoolean>();
        static readonly GrammarRule<YNull> Null = new GrammarRule<YNull>();
        static readonly GrammarRule<YNumber> Number = new GrammarRule<YNumber>();

        static readonly GrammarRule<YString> QuotedString = new GrammarRule<YString>();
        static readonly GrammarRule<YString> UnquotedString = new GrammarRule<YString>();
        static readonly GrammarRule<YString> AnyString = new GrammarRule<YString>();

        static readonly GrammarRule<YString> BlockScalarText = new GrammarRule<YString>();

        static readonly GrammarRule<YSequence> BlockSequence = new GrammarRule<YSequence>();
        static readonly GrammarRule<YSequence> FlowSequence = new GrammarRule<YSequence>();

        static readonly GrammarRule<YMapping> FlowMapping = new GrammarRule<YMapping>();
        static readonly GrammarRule<YMapping> BlockMapping = new GrammarRule<YMapping>();

        static readonly GrammarRule<YNode> FlowNode = new GrammarRule<YNode>();
        static readonly GrammarRule<YNode> BlockNode = new GrammarRule<YNode>();

        static readonly GrammarRule<YAlias> Alias = new GrammarRule<YAlias>();

        static readonly GrammarRule<YNode> Node = new GrammarRule<YNode>();

        static readonly GrammarRule<YNode> Yaml = new GrammarRule<YNode>();

        static YamlGrammar()
        {
            Boolean.Rule = Choice(
                YamlLexer.True.Constant(YBoolean.True),
                YamlLexer.False.Constant(YBoolean.False));

            Null.Rule = YamlLexer.Null.Constant(YNull.Null);
            Number.Rule = YamlLexer.Number.BindLexeme(YNumber.FromString);

            QuotedString.Rule = Choice(
                YamlLexer.SingleQuotedString.BindLexeme(YString.FromQuotedString),
                YamlLexer.DoubleQuotedString.BindLexeme(YString.FromQuotedString));

            UnquotedString.Rule = YamlLexer.UnquotedString.BindLexeme(YString.FromString);

            AnyString.Rule = Choice(QuotedString, UnquotedString);

            Alias.Rule = YamlLexer.Alias.BindLexeme(YAlias.FromString);

            BlockScalarText.Rule =
                from header in YamlLexer.BlockScalarHeader.Lexeme()
                from lines in Between(
                    ScopeTokenKind.ScopeBegin.Kind(),
                    OneOrMore(Choice(YamlLexer.BlockScalarLine.Lexeme(), YamlLexer.NewLine.Lexeme())),
                    ScopeTokenKind.ScopeEnd.Kind())
                select YString.FromBlockScalar(header, lines);

            BlockSequence.Rule =
                OneOrMore(YamlLexer.BlockSequenceItemMarker.Kind().Take(Node))
                .Bind(YSequence.FromList);

            FlowSequence.Rule =
                Between(YamlLexer.FlowSequenceBegin.Kind(), ZeroOrMore(FlowNode, YamlLexer.FlowItemSeparator.Kind()), YamlLexer.FlowSequenceEnd.Kind())
                .Bind(YSequence.FromList);

            BlockMapping.Rule =
                OneOrMore(NameValuePair(AnyString, YamlLexer.MappingSeparator.Kind(), Choice(Node, Alias)))
                .Bind(YMapping.FromDictionary);

            FlowMapping.Rule =
                Between(
                    YamlLexer.FlowMappingBegin.Kind(),
                    ZeroOrMore(
                        NameValuePair(
                            AnyString,
                            YamlLexer.MappingSeparator.Kind(),
                            Choice(FlowNode, Alias)),
                        YamlLexer.FlowItemSeparator.Kind()),
                    YamlLexer.FlowMappingEnd.Kind())
                .Bind(YMapping.FromDictionary);

            FlowNode.Rule =
                from anchor in Optional(YamlLexer.Anchor.Lexeme())
                from node in Choice<YNode>(
                    FlowSequence,
                    FlowMapping,
                    QuotedString,
                    Number,
                    Boolean,
                    Null,
                    UnquotedString)
                select YNode.AttachAnchor(node, anchor);

            BlockNode.Rule = Between(
                ScopeTokenKind.ScopeBegin.Kind(),
                from anchor in Optional(YamlLexer.Anchor.Lexeme())
                from node in Choice<YNode>(
                    BlockSequence,
                    BlockMapping,
                    QuotedString,
                    Number,
                    Boolean,
                    Null,
                    BlockScalarText,
                    UnquotedString)
                select YNode.AttachAnchor(node, anchor),
                ScopeTokenKind.ScopeEnd.Kind());

            Node.Rule = Choice(FlowNode, BlockNode);

            Yaml.Rule = OccupiesEntireInput(Node);
        }

        public YamlGrammar()
            : base("YAML", Yaml)
        { }
    }
}
