using Lexepars.Grammars.Yaml.Entities;
using Lexepars.OffsideRule;

namespace Lexepars.Grammars.Yaml
{
    public class DocumentGrammar : Grammar<YDocument>
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

        static readonly GrammarRule<YVersionDirective> YamlVersionDirective = new GrammarRule<YVersionDirective>();

        static readonly GrammarRule<YTagDirective> TagDirective = new GrammarRule<YTagDirective>();

        static readonly GrammarRule<YDirective> Directive = new GrammarRule<YDirective>();

        static readonly GrammarRule<YDocument> BareDocument = new GrammarRule<YDocument>();

        static readonly GrammarRule<YDocument> ExplicitDocument = new GrammarRule<YDocument>();

        static readonly GrammarRule<YDocument> DirectivesDocument = new GrammarRule<YDocument>();

        static readonly GrammarRule<YDocument> Document = new GrammarRule<YDocument>();

        static DocumentGrammar()
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
                    Attempt(FlowMapping), // enclosed into attempt lest it consumes QuotedString, UnquotedString
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
                    Attempt(BlockMapping), // enclosed into attempt lest it consumes QuotedString, UnquotedString
                    QuotedString,
                    Number,
                    Boolean,
                    Null,
                    BlockScalarText,
                    UnquotedString)
                select YNode.AttachAnchor(node, anchor),
                ScopeTokenKind.ScopeEnd.Kind());

            Node.Rule = Choice(FlowNode, BlockNode);

            TagDirective.Rule = YamlLexer.TagDirective.BindLexeme(YTagDirective.FromLexeme);

            YamlVersionDirective.Rule = YamlLexer.VersionDirective.BindLexeme(YVersionDirective.FromLexeme);

            Directive.Rule = Choice<YDirective>(TagDirective, YamlVersionDirective);

            BareDocument.Rule = Node.Bind(YDocument.FromBareNode);

            ExplicitDocument.Rule = YamlLexer.DirectivesEndMarker.Kind().Take(Optional(Node)).Bind(YDocument.FromBareNode);

            DirectivesDocument.Rule =
                from directives in OneOrMore(Directive)
                from body in YamlLexer.DirectivesEndMarker.Kind().Take(Node)
                select YDocument.Create(directives, body);

            Document.Rule =
                OccupiesEntireInput(Choice(DirectivesDocument, ExplicitDocument, BareDocument).ThenSkip(Optional(YamlLexer.DocumentEndMarker.Lexeme())));
        }

        public DocumentGrammar()
            : base("YAML", Document)
        {
        }
    }
}
