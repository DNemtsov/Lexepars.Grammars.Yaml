using Lexepars.TestFixtures;
using Lexepars.Grammars.Yaml.Entities;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Lexepars.Grammars.Yaml
{
    public class DocumentGrammarTests
    {
        static IEnumerable<Token> Tokenize(string input) => new YamlLexer().Tokenize(new StringReader(input));
        static readonly DocumentGrammar Yaml = new DocumentGrammar();

        [Fact]
        public void ParsesEmptyFlowMapping()
        {
            DocumentTestCases.EmptyFlowMapping.TestParser();
        }

        [Fact]
        public void ParsesEmptyFlowSequence()
        {
            DocumentTestCases.EmptyFlowSequence.TestParser();
        }

        [Fact]
        public void ParsesTrueLexeme()
        {
            var tokens = Tokenize("true");

            var doc = Yaml.Parses(tokens).ParsedValue;

            doc.Directives.ShouldBeEmpty();

            doc.Body.ShouldBe(YBoolean.True);
        }



        [Fact]
        public void ParsesFalseLexeme()
        {
            var tokens = Tokenize("false").ToArray();

            var doc = Yaml.Parses(tokens).ParsedValue;

            doc.Directives.ShouldBeEmpty();

            doc.Body.ShouldBe(YBoolean.False);
        }

        [Fact]
        public void ParsesNullLexeme()
        {
            var tokens = Tokenize("null");

            var doc = Yaml.Parses(tokens).ParsedValue;

            doc.Directives.ShouldBeEmpty();

            doc.Body.ShouldBe(YNull.Null);
        }

        [Fact]
        public void ParsesNumbers()
        {
            var tokens = Tokenize("10.123E-11");

            var doc = Yaml.Parses(tokens).ParsedValue;

            doc.Directives.ShouldBeEmpty();

            doc.Body.ShouldBe(new YNumber(10.123E-11m));
        }

        [Fact]
        public void ParsesFlowSequences()
        {
            var filled = Tokenize("[0, 1, .2]");

            var doc = Yaml.Parses(filled).ParsedValue;

            doc.Directives.ShouldBeEmpty();

            var sequence = doc.Body.ShouldBeOfType<YSequence>();

            sequence.ShouldBe(new[] { new YNumber(0m), new YNumber(1m), new YNumber(.2m) });
        }

        [Fact]
        public void ParsesFlowMapping()
        {
            var empty = Tokenize("{}");

            Yaml.Parses(empty).WithValue(doc =>
            {
                doc.Directives.ShouldBeEmpty();
                doc.Body.ShouldBeOfType<YMapping>().ShouldBeEmpty();
            });

            var filled = "{\"zero\" \n : \n 0, \"one\" \n \n\n: 1, \"two\" \n\n : 2}";

            Yaml.Parses(Tokenize(filled)).WithValue(doc =>
            {
                doc.Directives.ShouldBeEmpty();

                var mapping = doc.Body.ShouldBeOfType<YMapping>();

                mapping["zero"].ShouldBe(new YNumber(0m));
                mapping["one"].ShouldBe(new YNumber(1m));
                mapping["two"].ShouldBe(new YNumber(2m));
            });
        }

        [Fact]
        public void ParsesBlockSequenceL11L11L11()
        {
            DocumentTestCases.BlockSequenceL11L11L11.TestParser();
        }

        [Fact]
        public void ParsesUnquotedString()
        {
            DocumentTestCases.UnquotedString.TestParser();
        }

        [Fact]
        public void ParsesSpaceInterspercedFlowMapping()
        {
            DocumentTestCases.SpaceInterspercedFlowMapping.TestParser();
        }

        [Fact]
        public void ParsesHadlesSpaceInterspercedFlowSequencOfFlowMappings()
        {
            DocumentTestCases.SpaceInterspercedFlowSequencOfFlowMappings.TestParser();
        }

        [Fact]
        public void ParsesComplexFlow()
        {
            DocumentTestCases.ComplexFlow.TestParser();
        }

        [Fact]
        public void ParsesBlockSequenceL12L21()
        {
            DocumentTestCases.BlockSequenceL12L21.TestLexer();
        }

        [Fact]
        public void ParsesBlockMappingSequenceL11()
        {
            DocumentTestCases.BlockMappingSequenceL11.TestParser();
        }

        [Fact]
        public void ParsesBlockLiteralScalarKeep()
        {
            DocumentTestCases.BlockLiteralScalarKeep.TestParser();
        }

        [Fact]
        public void ParsesBlockLiteralScalarClip()
        {
            DocumentTestCases.BlockLiteralScalarClip.TestParser();
        }

        [Fact]
        public void ParsesBlockLiteralScalarStrip()
        {
            DocumentTestCases.BlockLiteralScalarStrip.TestParser();
        }

        [Fact]
        public void ParsesBlockFoldedScalarKeep()
        {
            DocumentTestCases.BlockFoldedScalarKeep.TestParser();
        }

        [Fact]
        public void ParsesBlockFoldedScalarClip()
        {
            DocumentTestCases.BlockFoldedScalarClip.TestParser();
        }

        [Fact]
        public void ParsesBlockFoldedScalarStrip()
        {
            DocumentTestCases.BlockFoldedScalarStrip.TestParser();
        }

        [Fact]
        public void ParsesBlockMappingInline()
        {
            DocumentTestCases.BlockMappingInline.TestParser();
        }

        [Fact]
        public void ParsesBlockMappingInlineAnchored()
        {
            DocumentTestCases.BlockMappingInlineAnchored.TestParser();
        }

        [Fact]
        public void ParsesBlockMappingInlineAnchoredWithAlias()
        {
            DocumentTestCases.BlockMappingInlineAnchoredWithAlias.TestParser();
        }

        [Fact]
        public void ParsesDirectivesDocument()
        {
            DocumentTestCases.DirectivesDocument.TestParser();
        }

        [Fact]
        public void ParsesDirectivesDocumentNoEndMarker()
        {
            DocumentTestCases.DirectivesDocumentNoEndMarker.TestParser();
        }

        [Fact]
        public void ParsesExplicitDocument()
        {
            DocumentTestCases.ExplicitDocument.TestParser();
        }

        [Fact]
        public void ParsesExplicitDocumentNoEndMarker()
        {
            DocumentTestCases.ExplicitDocumentNoEndMarker.TestParser();
        }

        [Fact]
        public void ParsesBareDocumentWithEndMarker()
        {
            DocumentTestCases.BareDocumentWithEndMarker.TestParser();
        }
    }
}