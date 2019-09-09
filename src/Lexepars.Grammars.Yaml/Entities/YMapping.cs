using System.Collections;
using System.Collections.Generic;

namespace Lexepars.Grammars.Yaml.Entities
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "By design.")]
    public class YMapping : YNode, IReadOnlyDictionary<string, YNode>
    {
        private readonly IReadOnlyDictionary<string, YNode> _dictionary;

        public YMapping(IReadOnlyDictionary<string, YNode> dictionary)
        {
            _dictionary = dictionary;
        }

        public YNode this[string key] => _dictionary[key];

        public IEnumerable<string> Keys => _dictionary.Keys;

        public IEnumerable<YNode> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        public bool ContainsKey(string key) => _dictionary.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, YNode>> GetEnumerator() => _dictionary.GetEnumerator();

        public bool TryGetValue(string key, out YNode value) => _dictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

        public static YMapping FromDictionary(IEnumerable<KeyValuePair<YString, YNode>> pairs)
        {
            var result = new Dictionary<string, YNode>();

            foreach (var pair in pairs)
                result[pair.Key.Value] = pair.Value;

            return new YMapping(result);
        }
    }
}
