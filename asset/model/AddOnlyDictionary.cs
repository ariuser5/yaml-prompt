using System.Collections;

namespace YamlPrompt.Model;

public class AddOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> where TKey : notnull
{
    private Dictionary<TKey, TValue> _dictionary = [];

    public TValue this[TKey key]
    {
        get { return _dictionary[key]; }
    }

    public IEnumerable<TKey> Keys => _dictionary.Keys;

    public IEnumerable<TValue> Values => _dictionary.Values;

    public int Count => _dictionary.Count;

    public void Add(TKey key, TValue value)
    {
        if (_dictionary.ContainsKey(key))
        {
            throw new ArgumentException("Key already exists in the dictionary.");
        }

        _dictionary.Add(key, value);
    }
    
    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }
    
    public bool TryAdd(TKey key, TValue value)
    {
        return _dictionary.TryAdd(key, value);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return _dictionary.TryGetValue(key, out value!);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}