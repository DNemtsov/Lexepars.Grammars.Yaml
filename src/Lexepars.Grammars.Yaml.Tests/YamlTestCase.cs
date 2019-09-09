using Lexepars.Grammars.Yaml.Entities;
using Lexepars.OffsideRule;
using Lexepars.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexepars.Grammars.Yaml
{
    class DocumentTestCase : LexerParserTestCase<YDocument>
    {
        public DocumentTestCase(string inputText, Token[] tokens, Action<YDocument> resultValidation)
            : base(inputText, tokens, resultValidation)
        { }

        protected override IEnumerable<Token> Tokenize(string inputText)
         => new YamlLexer().Tokenize(new StringReader(inputText));

        protected override IParser<YDocument> CreateParser()
         => new DocumentGrammar();

        public static DocumentTestCase YieldsSingleTokenInScope(string input, TokenKind kind, string lexeme = null)
        {
            return new DocumentTestCase(
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
}
