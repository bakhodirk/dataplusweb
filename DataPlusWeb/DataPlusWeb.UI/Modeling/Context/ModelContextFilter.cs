using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DataPlus.Web.UI
{
    public abstract class ModelContextFilter : IReadOnlyDictionary<string, object>
    {
        #region Private fields region

        private Dictionary<string, object> _fields = new();

        #endregion

        #region Protected methods region

        protected T Get<T>(string field) => (T?)_fields[field] ?? default!;

        protected void Set<T>(string field, T value) => _fields[field] = value!;

        #endregion

        #region IReadOnlyDictionary members

        bool IReadOnlyDictionary<string, object>.ContainsKey(string key) => _fields.ContainsKey(key);

        bool IReadOnlyDictionary<string, object>.TryGetValue(string key, [MaybeNullWhen(false)] out object value) => _fields.TryGetValue(key, out value);

        object IReadOnlyDictionary<string, object>.this[string key] => _fields[key];

        IEnumerable<string> IReadOnlyDictionary<string, object>.Keys => _fields.Keys;

        IEnumerable<object> IReadOnlyDictionary<string, object>.Values => _fields.Values;

        #endregion

        #region IReadOnlyCollection members

        int IReadOnlyCollection<KeyValuePair<string, object>>.Count => _fields.Count;

        #endregion

        #region IEnumerable members

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => _fields.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _fields.GetEnumerator();

        #endregion
    }
}
