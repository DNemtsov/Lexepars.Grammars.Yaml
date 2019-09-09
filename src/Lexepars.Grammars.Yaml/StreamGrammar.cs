using Lexepars.Grammars.Yaml.Entities;

namespace Lexepars.Grammars.Yaml
{
    public class StreamGrammar : Grammar<YStream>
    {
        static readonly DocumentGrammar Document = new DocumentGrammar();


        static readonly GrammarRule<YStream> Stream = new GrammarRule<YStream>();

        static StreamGrammar()
        {
            Stream.Rule =
                from startDoc in Document
                from docs in ZeroOrMore(
                    Choice(
                        YamlLexer.DocumentEndMarker.Kind().Take(Document),
                        Document))
                select YStream.FromDocs(startDoc, docs);
        }

        public StreamGrammar()
            : base("YAML Stream", Stream)
        { }
    }
}
