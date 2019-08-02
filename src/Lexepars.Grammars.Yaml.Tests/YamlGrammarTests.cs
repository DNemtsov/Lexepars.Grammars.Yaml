using Lexepars.TestFixtures;
using Lexepars.Tests.IntegrationTests.Yaml.Entities;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Lexepars.Tests.IntegrationTests.Yaml
{
    public class YamlGrammarTests
    {
        static IEnumerable<Token> Tokenize(string input) => new YamlLexer().Tokenize(new StringReader(input));
        static readonly YamlGrammar Yaml = new YamlGrammar();

        [Fact]
        public void ParsesEmptyFlowMapping()
        {
            YamlTestCases.EmptyFlowMapping.TestParser();
        }

        [Fact]
        public void ParsesEmptyFlowSequence()
        {
            YamlTestCases.EmptyFlowSequence.TestParser();
        }

        [Fact]
        public void ParsesTrueLexeme()
        {
            var tokens = Tokenize("true");

            Yaml.Parses(tokens).WithValue(YBoolean.True);
        }

        [Fact]
        public void ParsesFalseLexeme()
        {
            var tokens = Tokenize("false").ToArray();

            Yaml.Parses(tokens).WithValue(YBoolean.False);
        }

        [Fact]
        public void ParsesNullLexeme()
        {
            var tokens = Tokenize("null");

            Yaml.Parses(tokens).WithValue(YNull.Null);
        }

        [Fact]
        public void ParsesNumbers()
        {
            var tokens = Tokenize("10.123E-11");

            Yaml.Parses(tokens).WithValue(new YNumber(10.123E-11m));
        }

        [Fact]
        public void ParsesFlowSequences()
        {
            var filled = Tokenize("[0, 1, .2]");

            var sequence = Yaml.Parses(filled).ParsedValue.ShouldBeOfType<YSequence>();

            sequence.ShouldBe(new[] { new YNumber(0m), new YNumber(1m), new YNumber(.2m) });
        }

        [Fact]
        public void ParsesFlowMapping()
        {
            var empty = Tokenize("{}");

            Yaml.Parses(empty).WithValue(value => ((YMapping)value).Count.ShouldBe(0));

            var filled = "{\"zero\" \n : \n 0, \"one\" \n \n\n: 1, \"two\" \n\n : 2}";

            Yaml.Parses(Tokenize(filled)).WithValue(value =>
            {
                var dictionary = (YMapping) value;

                dictionary["zero"].ShouldBe(new YNumber(0m));
                dictionary["one"].ShouldBe(new YNumber(1m));
                dictionary["two"].ShouldBe(new YNumber(2m));
            });
        }

        [Fact]
        public void ParsesBlockSequenceL11L11L11()
        {
            YamlTestCases.BlockSequenceL11L11L11.TestParser();
        }

        [Fact]
        public void ParsesSpaceInterspercedFlowMapping()
        {
            YamlTestCases.SpaceInterspercedFlowMapping.TestParser();
        }

        [Fact]
        public void ParsesHadlesSpaceInterspercedFlowSequencOfFlowMappings()
        {
            YamlTestCases.SpaceInterspercedFlowSequencOfFlowMappings.TestParser();
        }

        [Fact]
        public void ParsesComplexFlow()
        {
            YamlTestCases.ComplexFlow.TestParser();
        }

        [Fact]
        public void ParsesBlockSequenceL12L21()
        {
            YamlTestCases.BlockSequenceL12L21.TestLexer();
        }

        [Fact]
        public void ParsesBlockMappingSequenceL11()
        {
            YamlTestCases.BlockMappingSequenceL11.TestParser();
        }

        [Fact]
        public void ParsesBlockLiteralScalarKeep()
        {
            YamlTestCases.BlockLiteralScalarKeep.TestParser();
        }

        [Fact]
        public void ParsesBlockLiteralScalarClip()
        {
            YamlTestCases.BlockLiteralScalarClip.TestParser();
        }

        [Fact]
        public void ParsesBlockLiteralScalarStrip()
        {
            YamlTestCases.BlockLiteralScalarStrip.TestParser();
        }

        [Fact]
        public void ParsesBlockFoldedScalarKeep()
        {
            YamlTestCases.BlockFoldedScalarKeep.TestParser();
        }

        [Fact]
        public void ParsesBlockFoldedScalarClip()
        {
            YamlTestCases.BlockFoldedScalarClip.TestParser();
        }

        [Fact]
        public void ParsesBlockFoldedScalarStrip()
        {
            YamlTestCases.BlockFoldedScalarStrip.TestParser();
        }

        [Fact]
        public void ParsesBlockMappingInline()
        {
            YamlTestCases.BlockMappingInline.TestParser();
        }

        [Fact]
        public void ParsesBlockMappingInlineAnchored()
        {
            YamlTestCases.BlockMappingInlineAnchored.TestParser();
        }

        [Fact]
        public void ParsesBlockMappingInlineAnchoredWithAlias()
        {
            YamlTestCases.BlockMappingInlineAnchoredWithAlias.TestParser();
        }
    }
}