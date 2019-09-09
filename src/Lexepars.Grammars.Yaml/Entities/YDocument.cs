using System;
using System.Collections.Generic;

namespace Lexepars.Grammars.Yaml.Entities
{
    public class YDocument
    {
        public IList<YDirective> Directives { get; }

        public YNode Body { get; }

        public YDocument(IList<YDirective> directives, YNode body)
        {
            Directives = directives;
            Body = body;
        }

        public static YDocument FromBareNode(YNode node) => new YDocument(Array.Empty<YDirective>(), node);

        public static YDocument Create(IList<YDirective> directives, YNode node) => new YDocument(directives, node);
    }
}
