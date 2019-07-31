namespace Lexepars.Tests.IntegrationTests.Yaml
{
    using Xunit;

    public class YamlLexerTests
    {
        [Fact]
        public void LexesEmptyFlowMapping()
        {
            YamlTestCases.EmptyFlowMapping.TestLexer();
        }

        [Fact]
        public void LexesEmptyFlowSequence()
        {
            YamlTestCases.EmptyFlowSequence.TestLexer();
        }

        [Fact]
        public void LexesNumbers()
        {
            foreach (var testCase in YamlTestCases.NumberTests)
                testCase.TestLexer();
        }

        [Fact]
        public void LexesUnquotedStings()
        {
            foreach (var testCase in YamlTestCases.UnquotedStringTests)
                testCase.TestLexer();
        }

        [Fact]
        public void LexesSpaceInterspercedFlowMapping()
        {
            YamlTestCases.SpaceInterspercedFlowMapping.TestLexer();
        }

        [Fact]
        public void LexesSpaceInterspercedFlowSequencOfFlowMappings()
        {
            YamlTestCases.SpaceInterspercedFlowSequencOfFlowMappings.TestLexer();
        }

        [Fact]
        public void LexesComplexFlow()
        {
            YamlTestCases.ComplexFlow.TestLexer();
        }

        [Fact]
        public void LexesBlockSequenceL11L11L11()
        {
            YamlTestCases.BlockSequenceL11L11L11.TestLexer();
        }

        [Fact]
        public void LexesBlockSequenceL12L21()
        {
            YamlTestCases.BlockSequenceL12L21.TestLexer();
        }

        [Fact]
        public void LexesBlockMappingSequenceL11()
        {
            YamlTestCases.BlockMappingSequenceL11.TestLexer();
        }

        [Fact]
        public void LexesBlockScalarHeader()
        {
            foreach (var testCase in YamlTestCases.BlockScalarHeaderTests)
                testCase.TestLexer();
        }

        [Fact]
        public void LexesBlockLiteralScalarKeep()
        {
            YamlTestCases.BlockLiteralScalarKeep.TestLexer();
        }

        [Fact]
        public void LexesBlockLiteralScalarClip()
        {
            YamlTestCases.BlockLiteralScalarClip.TestLexer();
        }

        [Fact]
        public void LexesBlockLiteralScalarStrip()
        {
            YamlTestCases.BlockLiteralScalarStrip.TestLexer();
        }

        [Fact]
        public void LexesBlockFoldedScalarKeep()
        {
            YamlTestCases.BlockFoldedScalarKeep.TestLexer();
        }

        [Fact]
        public void LexesBlockFoldedScalarClip()
        {
            YamlTestCases.BlockFoldedScalarClip.TestLexer();
        }

        [Fact]
        public void LexesBlockFoldedScalarStrip()
        {
            YamlTestCases.BlockFoldedScalarStrip.TestLexer();
        }

        [Fact]
        public void LexesBlockMappingInline()
        {
            YamlTestCases.BlockMappingInline.TestLexer();
        }

        [Fact]
        public void LexesBlockMappingInlineAnchored()
        {
            YamlTestCases.BlockMappingInlineAnchored.TestLexer();
        }

        [Fact]
        public void LexesBlockMappingInlineAnchoredWithAlias()
        {
            YamlTestCases.BlockMappingInlineAnchoredWithAlias.TestLexer();
        }
    }
}