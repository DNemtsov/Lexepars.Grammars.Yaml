using Lexepars.OffsideRule;
using Lexepars.Tests.Fixtures;
using Lexepars.Tests.IntegrationTests.Yaml.Entities;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexepars.Tests.IntegrationTests.Yaml
{
    class YamlTestCase : LexerParserTestCase<YNode>
    {
        public YamlTestCase(string inputText, Token[] tokens, Action<YNode> resultValidation)
            : base(inputText, tokens, resultValidation)
        { }

        protected override IEnumerable<Token> Tokenize(string inputText)
         => new YamlLexer().Tokenize(new StringReader(inputText));

        protected override IParser<YNode> CreateParser()
         => new YamlGrammar();

        public static YamlTestCase YieldsSingleTokenInScope(string input, TokenKind kind, string lexeme = null)
        {
            return new YamlTestCase(
                input,
                new[]
                {
                    new Token(ScopeTokenKind.ScopeBegin, new Position(1, 1), null),
                    new Token(kind, new Position(1, 1), lexeme ?? input),
                    new Token(ScopeTokenKind.ScopeEnd, new Position(1, input.Length + 1), null),
                },
                null
            );
        }
    }

    static class YamlTestCases
    {
        public static YamlTestCase EmptyFlowMapping = new YamlTestCase(
            "{}",
            new[]
            {
                CreateToken(1, 1, "{",  YamlLexer.FlowMappingBegin),
                CreateToken(1, 2, "}", YamlLexer.FlowMappingEnd),
            },
            parsed =>
            {
                parsed.ShouldBeOfType<YMapping>().ShouldBeEmpty();
            });

        public static YamlTestCase EmptyFlowSequence = new YamlTestCase(
           "[]",
           new[]
           {
                CreateToken(1, 1, "[",  YamlLexer.FlowSequenceBegin),
                CreateToken(1, 2, "]", YamlLexer.FlowSequenceEnd),
           },
           parsed =>
           {
               parsed.ShouldBeOfType<YSequence>().ShouldBeEmpty();
           });

        public static YamlTestCase SpaceInterspercedFlowMapping = new YamlTestCase(
            "  {\"zero\" \n" +
            " : \n 0, 'one' \n" +
            " \n" +
            "\n" +
            ": 1, 'two' " +
            "\n" +
            "\n" +
            "      : \"2\"}",
            new[]
            {
                CreateToken(1, 3, "{",  YamlLexer.FlowMappingBegin),
                CreateToken(1, 4, "\"zero\"", YamlLexer.DoubleQuotedString),
                CreateToken(2, 2, ":", YamlLexer.MappingSeparator),
                CreateToken(3, 2, "0", YamlLexer.Number),
                CreateToken(3, 3, ",", YamlLexer.FlowItemSeparator),
                CreateToken(3, 5, "'one'", YamlLexer.SingleQuotedString),
                CreateToken(6, 1, ":", YamlLexer.MappingSeparator),
                CreateToken(6, 3, "1", YamlLexer.Number),
                CreateToken(6, 4, ",", YamlLexer.FlowItemSeparator),
                CreateToken(6, 6, "'two'", YamlLexer.SingleQuotedString),
                CreateToken(8, 7, ":", YamlLexer.MappingSeparator),
                CreateToken(8, 9, "\"2\"", YamlLexer.DoubleQuotedString),
                CreateToken(8, 12, "}", YamlLexer.FlowMappingEnd),
            },
            parsed =>
            {
                var mapping = parsed.ShouldBeOfType<YMapping>();

                mapping["zero"].ShouldBe(new YNumber(0));
                mapping["one"].ShouldBe(new YNumber(1));
                mapping["two"].ShouldBe(new YString("2"));
            });

        public static YamlTestCase SpaceInterspercedFlowSequencOfFlowMappings = new YamlTestCase(
           "  [ {\"zero\" \n" +
           " : \n 0}, {'one' \n" +
           " \n" +
           "\n" +
           ": 1}, {   \"two\" " +
           "\n" +
           "\n" +
           "      : \"2\" }  ]      ",
           new[]
           {
                CreateToken(1, 3, "[",  YamlLexer.FlowSequenceBegin),
                CreateToken(1, 5, "{",  YamlLexer.FlowMappingBegin),
                CreateToken(1, 6, "\"zero\"", YamlLexer.DoubleQuotedString),
                CreateToken(2, 2, ":", YamlLexer.MappingSeparator),
                CreateToken(3, 2, "0", YamlLexer.Number),
                CreateToken(3, 3, "}",  YamlLexer.FlowMappingEnd),
                CreateToken(3, 4, ",", YamlLexer.FlowItemSeparator),
                CreateToken(3, 6, "{",  YamlLexer.FlowMappingBegin),
                CreateToken(3, 7, "'one'", YamlLexer.SingleQuotedString),
                CreateToken(6, 1, ":", YamlLexer.MappingSeparator),
                CreateToken(6, 3, "1", YamlLexer.Number),
                CreateToken(6, 4, "}",  YamlLexer.FlowMappingEnd),
                CreateToken(6, 5, ",", YamlLexer.FlowItemSeparator),
                CreateToken(6, 7, "{",  YamlLexer.FlowMappingBegin),
                CreateToken(6, 11, "\"two\"", YamlLexer.DoubleQuotedString),
                CreateToken(8, 7, ":", YamlLexer.MappingSeparator),
                CreateToken(8, 9, "\"2\"", YamlLexer.DoubleQuotedString),
                CreateToken(8, 13, "}",  YamlLexer.FlowMappingEnd),
                CreateToken(8, 16, "]", YamlLexer.FlowSequenceEnd),
           },
           parsed =>
           {
               var sequence = parsed.ShouldBeOfType<YSequence>();

               sequence[0].ShouldBeOfType<YMapping>()["zero"].ShouldBe(new YNumber(0));
               sequence[1].ShouldBeOfType<YMapping>()["one"].ShouldBe(new YNumber(1));
               sequence[2].ShouldBeOfType<YMapping>()["two"].ShouldBe(new YString("2"));
           });

        public static YamlTestCase ComplexFlow = new YamlTestCase(
           @"

                {
                    numbers: [1e+1, 20, -30],
                    ""window""    :
                    {
                        'title'    : Sample Widget,
                        parent: null,
                        ""maximized"": true,
                        ""transparent"": false
                    }
                }                   
    
            ",
            new[]
            {
                CreateToken(3, 17, "{",  YamlLexer.FlowMappingBegin),
                CreateToken(4, 21, "numbers",  YamlLexer.UnquotedString),
                CreateToken(4, 28, ":",  YamlLexer.MappingSeparator),
                CreateToken(4, 30 , "[",  YamlLexer.FlowSequenceBegin),
                CreateToken(4, 31, "1e+1",  YamlLexer.Number),
                CreateToken(4, 35, ",",  YamlLexer.FlowItemSeparator),
                CreateToken(4, 37, "20",  YamlLexer.Number),
                CreateToken(4, 39, ",",  YamlLexer.FlowItemSeparator),
                CreateToken(4, 41, "-30",  YamlLexer.Number),
                CreateToken(4, 44, "]",  YamlLexer.FlowSequenceEnd),
                CreateToken(4, 45, ",",  YamlLexer.FlowItemSeparator),
                CreateToken(5, 21, "\"window\"",  YamlLexer.DoubleQuotedString),
                CreateToken(5, 33, ":",  YamlLexer.MappingSeparator),
                CreateToken(6, 21, "{",  YamlLexer.FlowMappingBegin),
                CreateToken(7, 25, "'title'",  YamlLexer.SingleQuotedString),
                CreateToken(7, 36, ":",  YamlLexer.MappingSeparator),
                CreateToken(7, 38, "Sample Widget",  YamlLexer.UnquotedString),
                CreateToken(7, 51, ",",  YamlLexer.FlowItemSeparator),
                CreateToken(8, 25, "parent",  YamlLexer.UnquotedString),
                CreateToken(8, 31, ":",  YamlLexer.MappingSeparator),
                CreateToken(8, 33, "null",  YamlLexer.Null),
                CreateToken(8, 37, ",",  YamlLexer.FlowItemSeparator),
                CreateToken(9, 25, "\"maximized\"",  YamlLexer.DoubleQuotedString),
                CreateToken(9, 36, ":",  YamlLexer.MappingSeparator),
                CreateToken(9, 38, "true",  YamlLexer.True),
                CreateToken(9, 42, ",",  YamlLexer.FlowItemSeparator),
                CreateToken(10, 25, "\"transparent\"",  YamlLexer.DoubleQuotedString),
                CreateToken(10, 38, ":",  YamlLexer.MappingSeparator),
                CreateToken(10, 40, "false",  YamlLexer.False),
                CreateToken(11, 21, "}",  YamlLexer.FlowMappingEnd),
                CreateToken(12, 17, "}",  YamlLexer.FlowMappingEnd),
            },
            parsed =>
            {
                var mapping = parsed.ShouldBeOfType<YMapping>();

                mapping["numbers"]
                    .ShouldBeOfType<YSequence>()
                    .ShouldBe(new[] { new YNumber(10m), new YNumber(20m), new YNumber(-30m) });

                var window = mapping["window"].ShouldBeOfType<YMapping>();

                window["title"].ShouldBe(new YString("Sample Widget"));
                window["parent"].ShouldBe(YNull.Null);
                window["maximized"].ShouldBe(YBoolean.True);
                window["transparent"].ShouldBe(YBoolean.False);
            }
        );

        public static YamlTestCase BlockSequenceL11L11L11 = new YamlTestCase(
            "- a\n" +
            "- b    \n" +
            "- c",
            new[]
            {
                CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(1, 1, "- ", YamlLexer.BlockSequenceItemMarker),
                CreateToken(1, 3, "a", YamlLexer.UnquotedString),
                CreateToken(2, 1, "- ", YamlLexer.BlockSequenceItemMarker),
                CreateToken(2, 3, "b    ", YamlLexer.UnquotedString),
                CreateToken(3, 1, "- ", YamlLexer.BlockSequenceItemMarker),
                CreateToken(3, 3, "c", YamlLexer.UnquotedString),
                CreateToken(3, 4, null,  ScopeTokenKind.ScopeEnd),
            },
            parsed =>
            {
                var sequence = parsed.ShouldBeOfType<YSequence>();

                sequence.Count.ShouldBe(3);

                sequence[0].ShouldBe(new YString("a"));
                sequence[1].ShouldBe(new YString("b    "));
                sequence[2].ShouldBe(new YString("c"));
            });

        public static YamlTestCase BlockMappingSequenceL11 = new YamlTestCase(
            "sequence:\n" +
            "- a",
            new[]
            {
                CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(1, 1, "sequence", YamlLexer.UnquotedString),
                CreateToken(1, 9, ":", YamlLexer.MappingSeparator),
                CreateToken(2, 1, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(2, 1, "- ", YamlLexer.BlockSequenceItemMarker),
                CreateToken(2, 3, "a", YamlLexer.UnquotedString),
                CreateToken(2, 4, null,  ScopeTokenKind.ScopeEnd),
                CreateToken(2, 4, null,  ScopeTokenKind.ScopeEnd),
            },
            parsed =>
            {
                var mapping = parsed.ShouldBeOfType<YMapping>();

                var sequence = mapping["sequence"].ShouldBeOfType<YSequence>();

                sequence.Count.ShouldBe(1);

                sequence[0].ShouldBe(new YString("a"));
            });

        public static YamlTestCase BlockSequenceL12L21 = new YamlTestCase(
            "- - a\n" +
            "  - b\n",
            new[]
            {
                CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(1, 1, "- ", YamlLexer.BlockSequenceItemMarker),
                CreateToken(1, 3, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(1, 3, "- ", YamlLexer.BlockSequenceItemMarker),
                CreateToken(1, 5, "a", YamlLexer.UnquotedString),
                CreateToken(2, 3, "- ", YamlLexer.BlockSequenceItemMarker),
                CreateToken(2, 5, "b", YamlLexer.UnquotedString),
                CreateToken(2, 7, null,  ScopeTokenKind.ScopeEnd),
                CreateToken(2, 7, null,  ScopeTokenKind.ScopeEnd),
            },
            parsed =>
            {
                var sequence = parsed.ShouldBeOfType<YSequence>();
                sequence.Count.ShouldBe(2);

                var sequence11 = sequence[0].ShouldBeOfType<YSequence>();

                sequence11.Count.ShouldBe(1);

                sequence11[0].ShouldBe(new YString("a"));
                sequence11[1].ShouldBe(new YString("b"));
            });

        public static YamlTestCase BlockMappingInline = new YamlTestCase(
            "a: b",
            new[]
            {
                CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(1, 1, "a", YamlLexer.UnquotedString),
                CreateToken(1, 2, ":", YamlLexer.MappingSeparator),
                CreateToken(1, 4, "b", YamlLexer.UnquotedString),
                CreateToken(1, 5, null,  ScopeTokenKind.ScopeEnd),
            },
            parsed =>
            {
                var mapping = parsed.ShouldBeOfType<YMapping>();

                mapping["a"].ShouldBe(new YString("b"));
            });

        public static YamlTestCase BlockMappingInlineAnchored = new YamlTestCase(
            "&a123 a: b",
            new[]
            {
                CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(1, 1, "&a123", YamlLexer.Anchor),
                CreateToken(1, 7, "a", YamlLexer.UnquotedString),
                CreateToken(1, 8, ":", YamlLexer.MappingSeparator),
                CreateToken(1, 10, "b", YamlLexer.UnquotedString),
                CreateToken(1, 11, null,  ScopeTokenKind.ScopeEnd),
            },
            parsed =>
            {
                var mapping = parsed.ShouldBeOfType<YMapping>();

                mapping["a"].ShouldBe(new YString("b"));

                mapping.Anchor.ShouldBe("a123");
            });

        public static YamlTestCase BlockMappingInlineAnchoredWithAlias = new YamlTestCase(
           "&a123 a: *alias",
           new[]
           {
                CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(1, 1, "&a123", YamlLexer.Anchor),
                CreateToken(1, 7, "a", YamlLexer.UnquotedString),
                CreateToken(1, 8, ":", YamlLexer.MappingSeparator),
                CreateToken(1, 10, "*alias", YamlLexer.Alias),
                CreateToken(1, 16, null,  ScopeTokenKind.ScopeEnd),
           },
           parsed =>
           {
               var mapping = parsed.ShouldBeOfType<YMapping>();

               mapping["a"].ShouldBe(new YAlias("alias"));

               mapping.Anchor.ShouldBe("a123");
           });

        private static YamlTestCase SingleNumberToken(string input)
            => YamlTestCase.YieldsSingleTokenInScope(input, YamlLexer.Number);

        public static YamlTestCase[] NumberTests = new []
        {
            SingleNumberToken("0"),
            SingleNumberToken("-0"),
            SingleNumberToken("1"),
            SingleNumberToken("-1"),
            SingleNumberToken("12345"),
            SingleNumberToken("-12345"),
            SingleNumberToken("0.012"),
            SingleNumberToken("-0.012"),
            SingleNumberToken(".012"),
            SingleNumberToken("-.012"),
            SingleNumberToken("0e1"),
            SingleNumberToken("-0e1"),
            SingleNumberToken("0e+1"),
            SingleNumberToken("-0e+1"),
            SingleNumberToken("0e-1"),
            SingleNumberToken("-0e-1"),
            SingleNumberToken("0E1"),
            SingleNumberToken("-0E1"),
            SingleNumberToken("0E+1"),
            SingleNumberToken("-0E+1"),
            SingleNumberToken("0E-1"),
            SingleNumberToken("-0E-1"),
            SingleNumberToken("10e11"),
            SingleNumberToken("-10e17"),
            SingleNumberToken("10.123e14"),
            SingleNumberToken("-10.123e16"),
        };

        public static YamlTestCase[] UnquotedStringTests = new []
        {
            YamlTestCase.YieldsSingleTokenInScope("something123", YamlLexer.UnquotedString),
            YamlTestCase.YieldsSingleTokenInScope("unquoted-string", YamlLexer.UnquotedString),
            YamlTestCase.YieldsSingleTokenInScope("$%__+=|", YamlLexer.UnquotedString),
            YamlTestCase.YieldsSingleTokenInScope("white   space", YamlLexer.UnquotedString),
            YamlTestCase.YieldsSingleTokenInScope("more white   space   ", YamlLexer.UnquotedString),
            new YamlTestCase(
                "something\r",
                new[]
                {
                    CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                    CreateToken(1, 1, "something", YamlLexer.UnquotedString),
                    CreateToken(1, 11, null,  ScopeTokenKind.ScopeEnd),
                },
                null),
            new YamlTestCase(
                "some    \r",
                new[]
                {
                    CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                    CreateToken(1, 1, "some    ", YamlLexer.UnquotedString),
                    CreateToken(1, 10, null,  ScopeTokenKind.ScopeEnd),
                },
                null),
            new YamlTestCase(
                "some    \n",
                new[]
                {
                    CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                    CreateToken(1, 1, "some    ", YamlLexer.UnquotedString),
                    CreateToken(1, 10, null,  ScopeTokenKind.ScopeEnd),
                },
                null),
             new YamlTestCase(
                "&sdf some    \n",
                new[]
                {
                    CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                    CreateToken(1, 1, "&sdf", YamlLexer.Anchor),
                    CreateToken(1, 6, "some    ", YamlLexer.UnquotedString),
                    CreateToken(1, 15, null,  ScopeTokenKind.ScopeEnd),
                },
                null),
        };

        public static YamlTestCase[] BlockScalarHeaderTests = new[]
        {
            YamlTestCase.YieldsSingleTokenInScope(">\n", YamlLexer.BlockScalarHeader, ">"),
            YamlTestCase.YieldsSingleTokenInScope("|\n", YamlLexer.BlockScalarHeader, "|"),
            YamlTestCase.YieldsSingleTokenInScope(">+\r\n", YamlLexer.BlockScalarHeader, ">+"),
            YamlTestCase.YieldsSingleTokenInScope("|+\r", YamlLexer.BlockScalarHeader, "|+"),
            YamlTestCase.YieldsSingleTokenInScope(">-\r", YamlLexer.BlockScalarHeader, ">-"),
            YamlTestCase.YieldsSingleTokenInScope("|-\n", YamlLexer.BlockScalarHeader, "|-"),

            YamlTestCase.YieldsSingleTokenInScope(">   \n", YamlLexer.BlockScalarHeader, ">"),
            YamlTestCase.YieldsSingleTokenInScope("|  \n", YamlLexer.BlockScalarHeader, "|"),
            YamlTestCase.YieldsSingleTokenInScope(">+ \r\n", YamlLexer.BlockScalarHeader, ">+"),
            YamlTestCase.YieldsSingleTokenInScope("|+   \r", YamlLexer.BlockScalarHeader, "|+"),
            YamlTestCase.YieldsSingleTokenInScope(">-    \r", YamlLexer.BlockScalarHeader, ">-"),
            YamlTestCase.YieldsSingleTokenInScope("|-\n", YamlLexer.BlockScalarHeader, "|-"),


            YamlTestCase.YieldsSingleTokenInScope(">1\n", YamlLexer.BlockScalarHeader, ">1"),
            YamlTestCase.YieldsSingleTokenInScope("|4\n", YamlLexer.BlockScalarHeader, "|4"),
            YamlTestCase.YieldsSingleTokenInScope(">+1\n", YamlLexer.BlockScalarHeader, ">+1"),
            YamlTestCase.YieldsSingleTokenInScope("|+4\n", YamlLexer.BlockScalarHeader, "|+4"),
            YamlTestCase.YieldsSingleTokenInScope(">1+\n", YamlLexer.BlockScalarHeader, ">1+"),
            YamlTestCase.YieldsSingleTokenInScope("|4+\n", YamlLexer.BlockScalarHeader, "|4+"),
            YamlTestCase.YieldsSingleTokenInScope(">-1\n", YamlLexer.BlockScalarHeader, ">-1"),
            YamlTestCase.YieldsSingleTokenInScope("|-4\n", YamlLexer.BlockScalarHeader, "|-4"),
            YamlTestCase.YieldsSingleTokenInScope(">1-\n", YamlLexer.BlockScalarHeader, ">1-"),
            YamlTestCase.YieldsSingleTokenInScope("|4-\n", YamlLexer.BlockScalarHeader, "|4-"),

            YamlTestCase.YieldsSingleTokenInScope(">1  \n", YamlLexer.BlockScalarHeader, ">1"),
            YamlTestCase.YieldsSingleTokenInScope("|4  \n", YamlLexer.BlockScalarHeader, "|4"),
            YamlTestCase.YieldsSingleTokenInScope(">+1 \r", YamlLexer.BlockScalarHeader, ">+1"),
            YamlTestCase.YieldsSingleTokenInScope("|+4  \r\n", YamlLexer.BlockScalarHeader, "|+4"),
            YamlTestCase.YieldsSingleTokenInScope(">1+ \n", YamlLexer.BlockScalarHeader, ">1+"),
            YamlTestCase.YieldsSingleTokenInScope("|4+   \n", YamlLexer.BlockScalarHeader, "|4+"),
            YamlTestCase.YieldsSingleTokenInScope(">-1 \n", YamlLexer.BlockScalarHeader, ">-1"),
            YamlTestCase.YieldsSingleTokenInScope("|-4   \n", YamlLexer.BlockScalarHeader, "|-4"),
            YamlTestCase.YieldsSingleTokenInScope(">1-  \n", YamlLexer.BlockScalarHeader, ">1-"),
            YamlTestCase.YieldsSingleTokenInScope("|4- \n", YamlLexer.BlockScalarHeader, "|4-"),

            YamlTestCase.YieldsSingleTokenInScope(">32\n", YamlLexer.BlockScalarHeader, ">32"),
            YamlTestCase.YieldsSingleTokenInScope("|56\n", YamlLexer.BlockScalarHeader, "|56"),
            YamlTestCase.YieldsSingleTokenInScope(">+32\n", YamlLexer.BlockScalarHeader, ">+32"),
            YamlTestCase.YieldsSingleTokenInScope("|+56\n", YamlLexer.BlockScalarHeader, "|+56"),
            YamlTestCase.YieldsSingleTokenInScope(">32+\n", YamlLexer.BlockScalarHeader, ">32+"),
            YamlTestCase.YieldsSingleTokenInScope("|56+\n", YamlLexer.BlockScalarHeader, "|56+"),
            YamlTestCase.YieldsSingleTokenInScope(">-32\n", YamlLexer.BlockScalarHeader, ">-32"),
            YamlTestCase.YieldsSingleTokenInScope("|-56\n", YamlLexer.BlockScalarHeader, "|-56"),
            YamlTestCase.YieldsSingleTokenInScope(">32-\n", YamlLexer.BlockScalarHeader, ">32-"),
            YamlTestCase.YieldsSingleTokenInScope("|56-\n", YamlLexer.BlockScalarHeader, "|56-"),

            YamlTestCase.YieldsSingleTokenInScope(">32  \n", YamlLexer.BlockScalarHeader, ">32"),
            YamlTestCase.YieldsSingleTokenInScope("|56  \n", YamlLexer.BlockScalarHeader, "|56"),
            YamlTestCase.YieldsSingleTokenInScope(">+32 \r", YamlLexer.BlockScalarHeader, ">+32"),
            YamlTestCase.YieldsSingleTokenInScope("|+56  \r\n", YamlLexer.BlockScalarHeader, "|+56"),
            YamlTestCase.YieldsSingleTokenInScope(">32+ \n", YamlLexer.BlockScalarHeader, ">32+"),
            YamlTestCase.YieldsSingleTokenInScope("|56+   \n", YamlLexer.BlockScalarHeader, "|56+"),
            YamlTestCase.YieldsSingleTokenInScope(">-32 \n", YamlLexer.BlockScalarHeader, ">-32"),
            YamlTestCase.YieldsSingleTokenInScope("|-56   \n", YamlLexer.BlockScalarHeader, "|-56"),
            YamlTestCase.YieldsSingleTokenInScope(">32-  \n", YamlLexer.BlockScalarHeader, ">32-"),
            YamlTestCase.YieldsSingleTokenInScope("|56- \n", YamlLexer.BlockScalarHeader, "|56-"),

            new YamlTestCase(
                ">32  # Comment",
                new[]
                {
                    CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                    CreateToken(1, 1, ">32", YamlLexer.BlockScalarHeader),
                    CreateToken(1, 15, null,  ScopeTokenKind.ScopeEnd),
                },
                null),
            new YamlTestCase(
                "|45+#",
                new[]
                {
                    CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                    CreateToken(1, 1, "|45+", YamlLexer.BlockScalarHeader),
                    CreateToken(1, 6, null,  ScopeTokenKind.ScopeEnd),
                },
                null),
            new YamlTestCase(
                "&anchor |45+#",
                new[]
                {
                    CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                    CreateToken(1, 1, "&anchor", YamlLexer.Anchor),
                    CreateToken(1, 9, "|45+", YamlLexer.BlockScalarHeader),
                    CreateToken(1, 14, null,  ScopeTokenKind.ScopeEnd),
                },
                null),

        };

        public static YamlTestCase BlockLiteralScalarKeep = new YamlTestCase(
           "|2+\n" +
           "    block literal scalar with\n" +
           "  something: looking\n" +
           "  legit: [flow, sequence]\n" +
           "     or: {flow: mapping}   \r\n" +
           "   # or even a comment\n" +
           "\n" +
           "           \n",
           new[]
           {
                CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(1, 1, "|2+", YamlLexer.BlockScalarHeader),
                CreateToken(2, 3, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(2, 3, "  block literal scalar with\n", YamlLexer.BlockScalarLine),
                CreateToken(3, 3, "something: looking\n", YamlLexer.BlockScalarLine),
                CreateToken(4, 3, "legit: [flow, sequence]\n", YamlLexer.BlockScalarLine),
                CreateToken(5, 3, "   or: {flow: mapping}   \r\n", YamlLexer.BlockScalarLine),
                CreateToken(6, 3, " # or even a comment\n", YamlLexer.BlockScalarLine),
                CreateToken(7, 1, "\n",  YamlLexer.NewLine),
                CreateToken(8, 3, "         \n", YamlLexer.BlockScalarLine),
                CreateToken(8, 13, null,  ScopeTokenKind.ScopeEnd),
                CreateToken(8, 13, null,  ScopeTokenKind.ScopeEnd)
           },
           parsed =>
           {
               var plockScalarText = parsed.ShouldBeOfType<YString>();

               plockScalarText.Value.ShouldBe(
                   "  block literal scalar with\n" +
                   "something: looking\n" +
                   "legit: [flow, sequence]\n" +
                   "   or: {flow: mapping}   \n" +
                   " # or even a comment\n" +
                   "\n" +
                   "         \n");
           });

        public static YamlTestCase BlockLiteralScalarClip = new YamlTestCase(
            "|2\n" +
            "    block literal scalar with\n" +
            "  something: looking\n" +
            "  legit: [flow, sequence]\n" +
            "     or: {flow: mapping}   \r\n" +
            "   # or even a comment\n" +
            "\n" +
            "           \n",
            new[]
            {
                CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(1, 1, "|2", YamlLexer.BlockScalarHeader),
                CreateToken(2, 3, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(2, 3, "  block literal scalar with\n", YamlLexer.BlockScalarLine),
                CreateToken(3, 3, "something: looking\n", YamlLexer.BlockScalarLine),
                CreateToken(4, 3, "legit: [flow, sequence]\n", YamlLexer.BlockScalarLine),
                CreateToken(5, 3, "   or: {flow: mapping}   \r\n", YamlLexer.BlockScalarLine),
                CreateToken(6, 3, " # or even a comment\n", YamlLexer.BlockScalarLine),
                CreateToken(7, 1, "\n",  YamlLexer.NewLine),
                CreateToken(8, 3, "         \n", YamlLexer.BlockScalarLine),
                CreateToken(8, 13, null,  ScopeTokenKind.ScopeEnd),
                CreateToken(8, 13, null,  ScopeTokenKind.ScopeEnd)
            },
            parsed =>
            {
                var plockScalarText = parsed.ShouldBeOfType<YString>();

                plockScalarText.Value.ShouldBe(
                    "  block literal scalar with\n" +
                    "something: looking\n" +
                    "legit: [flow, sequence]\n" +
                    "   or: {flow: mapping}   \n" +
                    " # or even a comment\n");
            });

        public static YamlTestCase BlockLiteralScalarStrip = new YamlTestCase(
            "|2-\n" +
            "    block literal scalar with\n" +
            "  something: looking\n" +
            "  legit: [flow, sequence]\n" +
            "     or: {flow: mapping}   \r\n" +
            "   # or even a comment\n" +
            "\n" +
            "           \n",
            new[]
            {
                CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(1, 1, "|2-", YamlLexer.BlockScalarHeader),
                CreateToken(2, 3, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(2, 3, "  block literal scalar with\n", YamlLexer.BlockScalarLine),
                CreateToken(3, 3, "something: looking\n", YamlLexer.BlockScalarLine),
                CreateToken(4, 3, "legit: [flow, sequence]\n", YamlLexer.BlockScalarLine),
                CreateToken(5, 3, "   or: {flow: mapping}   \r\n", YamlLexer.BlockScalarLine),
                CreateToken(6, 3, " # or even a comment\n", YamlLexer.BlockScalarLine),
                CreateToken(7, 1, "\n",  YamlLexer.NewLine),
                CreateToken(8, 3, "         \n", YamlLexer.BlockScalarLine),
                CreateToken(8, 13, null,  ScopeTokenKind.ScopeEnd),
                CreateToken(8, 13, null,  ScopeTokenKind.ScopeEnd)
            },
            parsed =>
            {
                var plockScalarText = parsed.ShouldBeOfType<YString>();

                plockScalarText.Value.ShouldBe(
                    "  block literal scalar with\n" +
                    "something: looking\n" +
                    "legit: [flow, sequence]\n" +
                    "   or: {flow: mapping}   \n" +
                    " # or even a comment");
            });

        public static YamlTestCase BlockFoldedScalarKeep = new YamlTestCase(
            ">2+\n" +
            "  folded scalar with\n" +
            "  something: looking\n" +
            "\n" +
            "  legit: [flow, sequence]\n" +
            "     more indent   \r\n" +
            "     # keeps the new lines\n" +
            "\n" +
            "           \n",
            new[]
            {
                CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(1, 1, ">2+", YamlLexer.BlockScalarHeader),
                CreateToken(2, 3, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(2, 3, "folded scalar with\n", YamlLexer.BlockScalarLine),
                CreateToken(3, 3, "something: looking\n", YamlLexer.BlockScalarLine),
                CreateToken(4, 1, "\n",  YamlLexer.NewLine),
                CreateToken(5, 3, "legit: [flow, sequence]\n", YamlLexer.BlockScalarLine),
                CreateToken(6, 3, "   more indent   \r\n", YamlLexer.BlockScalarLine),
                CreateToken(7, 3, "   # keeps the new lines\n", YamlLexer.BlockScalarLine),
                CreateToken(8, 1, "\n",  YamlLexer.NewLine),
                CreateToken(9, 3, "         \n", YamlLexer.BlockScalarLine),
                CreateToken(9, 13, null,  ScopeTokenKind.ScopeEnd),
                CreateToken(9, 13, null,  ScopeTokenKind.ScopeEnd)
            },
            parsed =>
            {
                var plockScalarText = parsed.ShouldBeOfType<YString>();

                plockScalarText.Value.ShouldBe(
                    "folded scalar with something: looking\n" +
                    "legit: [flow, sequence]\n" +
                    "   more indent   \n" +
                    "   # keeps the new lines\n" +
                    "\n" +
                    "\n");
            });

        public static YamlTestCase BlockFoldedScalarClip = new YamlTestCase(
            ">2\n" +
            "  folded scalar with\n" +
            "  something: looking\n" +
            "\n" +
            "  legit: [flow, sequence]\n" +
            "     more indent   \r\n" +
            "     # keeps the new lines\n" +
            "\n" +
            "           \n",
            new[]
            {
                CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(1, 1, ">2", YamlLexer.BlockScalarHeader),
                CreateToken(2, 3, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(2, 3, "folded scalar with\n", YamlLexer.BlockScalarLine),
                CreateToken(3, 3, "something: looking\n", YamlLexer.BlockScalarLine),
                CreateToken(4, 1, "\n",  YamlLexer.NewLine),
                CreateToken(5, 3, "legit: [flow, sequence]\n", YamlLexer.BlockScalarLine),
                CreateToken(6, 3, "   more indent   \r\n", YamlLexer.BlockScalarLine),
                CreateToken(7, 3, "   # keeps the new lines\n", YamlLexer.BlockScalarLine),
                CreateToken(8, 1, "\n",  YamlLexer.NewLine),
                CreateToken(9, 3, "         \n", YamlLexer.BlockScalarLine),
                CreateToken(9, 13, null,  ScopeTokenKind.ScopeEnd),
                CreateToken(9, 13, null,  ScopeTokenKind.ScopeEnd)
            },
            parsed =>
            {
                var plockScalarText = parsed.ShouldBeOfType<YString>();

                plockScalarText.Value.ShouldBe(
                    "folded scalar with " +
                    "something: looking" +
                    "\n" +
                    "legit: [flow, sequence]\n" +
                    "   more indent   \n" +
                    "   # keeps the new lines\n");
            });

        public static YamlTestCase BlockFoldedScalarStrip = new YamlTestCase(
            ">2-\n" +
            "  folded scalar with\n" +
            "  something: looking\n" +
            "\n" +
            "  legit: [flow, sequence]\n" +
            "     more indent   \r\n" +
            "     # keeps the new lines\n" +
            "\n" +
            "           \n",
            new[]
            {
                CreateToken(1, 1, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(1, 1, ">2-", YamlLexer.BlockScalarHeader),
                CreateToken(2, 3, null,  ScopeTokenKind.ScopeBegin),
                CreateToken(2, 3, "folded scalar with\n", YamlLexer.BlockScalarLine),
                CreateToken(3, 3, "something: looking\n", YamlLexer.BlockScalarLine),
                CreateToken(4, 1, "\n",  YamlLexer.NewLine),
                CreateToken(5, 3, "legit: [flow, sequence]\n", YamlLexer.BlockScalarLine),
                CreateToken(6, 3, "   more indent   \r\n", YamlLexer.BlockScalarLine),
                CreateToken(7, 3, "   # keeps the new lines\n", YamlLexer.BlockScalarLine),
                CreateToken(8, 1, "\n",  YamlLexer.NewLine),
                CreateToken(9, 3, "         \n", YamlLexer.BlockScalarLine),
                CreateToken(9, 13, null,  ScopeTokenKind.ScopeEnd),
                CreateToken(9, 13, null,  ScopeTokenKind.ScopeEnd)
            },
            parsed =>
            {
                var plockScalarText = parsed.ShouldBeOfType<YString>();

                plockScalarText.Value.ShouldBe(
                        "folded scalar with " +
                    "something: looking" +
                    "\n" +
                    "legit: [flow, sequence]\n" +
                    "   more indent   \n" +
                    "   # keeps the new lines");
            });

        private static Token CreateToken(int line, int column, string lexeme, TokenKind tokenKind)
        {
            return new Token(tokenKind, new Position(line, column), lexeme);
        }
    }
}
