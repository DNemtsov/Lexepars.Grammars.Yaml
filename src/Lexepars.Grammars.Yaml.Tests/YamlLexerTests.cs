namespace Lexepars.Grammars.Yaml
{
    using Xunit;

    public class YamlLexerTests
    {
        [Fact]
        public void LexesEmptyFlowMapping()
        {
            DocumentTestCases.EmptyFlowMapping.TestLexer();
        }

        [Fact]
        public void LexesEmptyFlowSequence()
        {
            DocumentTestCases.EmptyFlowSequence.TestLexer();
        }

        [Fact]
        public void LexesNumbers()
        {
            foreach (var testCase in DocumentTestCases.NumberTests)
                testCase.TestLexer();
        }

        [Fact]
        public void LexesUnquotedStings()
        {
            foreach (var testCase in DocumentTestCases.UnquotedStringTests)
                testCase.TestLexer();
        }

        [Fact]
        public void LexesSpaceInterspercedFlowMapping()
        {
            DocumentTestCases.SpaceInterspercedFlowMapping.TestLexer();
        }

        [Fact]
        public void LexesSpaceInterspercedFlowSequencOfFlowMappings()
        {
            DocumentTestCases.SpaceInterspercedFlowSequencOfFlowMappings.TestLexer();
        }

        [Fact]
        public void LexesComplexFlow()
        {
            DocumentTestCases.ComplexFlow.TestLexer();
        }

        [Fact]
        public void LexesBlockSequenceL11L11L11()
        {
            DocumentTestCases.BlockSequenceL11L11L11.TestLexer();
        }

        [Fact]
        public void LexesBlockSequenceL12L21()
        {
            DocumentTestCases.BlockSequenceL12L21.TestLexer();
        }

        [Fact]
        public void LexesBlockMappingSequenceL11()
        {
            DocumentTestCases.BlockMappingSequenceL11.TestLexer();
        }

        [Fact]
        public void LexesBlockScalarHeader()
        {
            foreach (var testCase in DocumentTestCases.BlockScalarHeaderTests)
                testCase.TestLexer();
        }

        [Fact]
        public void LexesBlockLiteralScalarKeep()
        {
            DocumentTestCases.BlockLiteralScalarKeep.TestLexer();
        }

        [Fact]
        public void LexesBlockLiteralScalarClip()
        {
            DocumentTestCases.BlockLiteralScalarClip.TestLexer();
        }

        [Fact]
        public void LexesBlockLiteralScalarStrip()
        {
            DocumentTestCases.BlockLiteralScalarStrip.TestLexer();
        }

        [Fact]
        public void LexesBlockFoldedScalarKeep()
        {
            DocumentTestCases.BlockFoldedScalarKeep.TestLexer();
        }

        [Fact]
        public void LexesBlockFoldedScalarClip()
        {
            DocumentTestCases.BlockFoldedScalarClip.TestLexer();
        }

        [Fact]
        public void LexesBlockFoldedScalarStrip()
        {
            DocumentTestCases.BlockFoldedScalarStrip.TestLexer();
        }

        [Fact]
        public void LexesBlockMappingInline()
        {
            DocumentTestCases.BlockMappingInline.TestLexer();
        }

        [Fact]
        public void LexesBlockMappingInlineAnchored()
        {
            DocumentTestCases.BlockMappingInlineAnchored.TestLexer();
        }

        [Fact]
        public void LexesBlockMappingInlineAnchoredWithAlias()
        {
            DocumentTestCases.BlockMappingInlineAnchoredWithAlias.TestLexer();
        }

        [Fact]
        public void LexesDirectivesDocument()
        {
            DocumentTestCases.DirectivesDocument.TestLexer();
        }

        [Fact]
        public void LexesDirectivesDocumentNoEndMarker()
        {
            DocumentTestCases.DirectivesDocumentNoEndMarker.TestLexer();
        }

        [Fact]
        public void LexesExplicitDocument()
        {
            DocumentTestCases.ExplicitDocument.TestLexer();
        }

        [Fact]
        public void LexesExplicitDocumentNoEndMarker()
        {
            DocumentTestCases.ExplicitDocumentNoEndMarker.TestLexer();
        }

        [Fact]
        public void LexesBareDocumentWithEndMarker()
        {
            DocumentTestCases.BareDocumentWithEndMarker.TestLexer();
        }
    }
}