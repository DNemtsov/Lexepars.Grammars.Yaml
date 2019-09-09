using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lexepars.Grammars.Yaml.Entities
{
    public class YStream : IList<YDocument>
    {
        private readonly IList<YDocument> _documents;

        public YStream(IList<YDocument> documents)
        {
            _documents = documents;
        }

        public static YStream FromDocs(YDocument startDoc, IList<YDocument> subsequentDocs) => new YStream(new[] { startDoc }.Concat(subsequentDocs).ToList());

        public YDocument this[int index] { get => _documents[index]; set => _documents[index] = value; }

        public int Count => _documents.Count;

        public bool IsReadOnly => _documents.IsReadOnly;

        public void Add(YDocument item) => _documents.Add(item);

        public void Clear() => _documents.Clear();

        public bool Contains(YDocument item) => _documents.Contains(item);

        public void CopyTo(YDocument[] array, int arrayIndex) => _documents.CopyTo(array, arrayIndex);

        public IEnumerator<YDocument> GetEnumerator() => _documents.GetEnumerator();

        public int IndexOf(YDocument item) => _documents.IndexOf(item);

        public void Insert(int index, YDocument item) => _documents.Insert(index, item);

        public bool Remove(YDocument item) => _documents.Remove(item);

        public void RemoveAt(int index) => _documents.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => _documents.GetEnumerator();
    }
}
