using System.Collections;
using System.Collections.Generic;

namespace Lexepars.Grammars.Yaml.Entities
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "By design.")]
    public class YSequence : YNode, IReadOnlyList<YNode>
    {
        private readonly IList<YNode> _items;

        public YSequence(IList<YNode> items)
        {
            _items = items;
        }

        public static YSequence FromList(IList<YNode> items) => new YSequence(items);

        public YNode this[int index] => _items[index];

        public int Count => _items.Count;

        public IEnumerator<YNode> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}
