using System.Collections;

namespace YamlPrompt.Shared.Collections;

/// <summary>
/// Represents a dictionary that allows adding new key-value pairs, but does not allow updating or removing entries.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
public class AddOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> where TKey : notnull
{
    private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    public TValue this[TKey key]
    {
        get { return _dictionary[key]; }
    }

    /// <summary>
    /// Gets an enumerable collection containing the keys in the dictionary.
    /// </summary>
    public IEnumerable<TKey> Keys => _dictionary.Keys;

    /// <summary>
    /// Gets an enumerable collection containing the values in the dictionary.
    /// </summary>
    public IEnumerable<TValue> Values => _dictionary.Values;

    /// <summary>
    /// Gets the number of key-value pairs contained in the dictionary.
    /// </summary>
    public int Count => _dictionary.Count;

    /// <summary>
    /// Adds a new key-value pair to the dictionary. Throws if the key already exists.
    /// </summary>
    public void Add(TKey key, TValue value)
    {
        if (_dictionary.ContainsKey(key))
        {
            throw new ArgumentException("Key already exists in the dictionary.");
        }

        _dictionary.Add(key, value);
    }
    
    /// <summary>
    /// Determines whether the dictionary contains the specified key.
    /// </summary>
    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }
    
    /// <summary>
    /// Attempts to add the specified key and value to the dictionary. Returns true if added, false if the key already exists.
    /// </summary>
    public bool TryAdd(TKey key, TValue value)
    {
        return _dictionary.TryAdd(key, value);
    }

    /// <summary>
    /// Gets the value associated with the specified key, if it exists.
    /// </summary>
    public bool TryGetValue(TKey key, out TValue value)
    {
        return _dictionary.TryGetValue(key, out value!);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the dictionary.
    /// </summary>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}