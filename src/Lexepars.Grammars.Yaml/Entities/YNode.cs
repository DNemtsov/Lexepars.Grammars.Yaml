using System;

namespace Lexepars.Tests.IntegrationTests.Yaml.Entities
{
    public class YNode
    {
        public string Anchor { get; set; }

        public static YNode AttachAnchor(YNode node, string anchor)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (anchor != null)
                node.Anchor = anchor.Substring(1);

            return node;
        }
    }
}
