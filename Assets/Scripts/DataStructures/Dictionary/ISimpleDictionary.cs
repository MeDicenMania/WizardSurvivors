using ED262C;

public interface ISimpleDictionary<TKey, TValue>
{
    int Count { get; }

    TValue this[TKey key] { get; set; }

    void Add(TKey key, TValue value);

    bool Remove(TKey key);

    bool ContainsKey(TKey key);

    bool TryGetValue(TKey key, out TValue value);

    void Clear();

    ISimpleList<TKey> GetKeys();

    ISimpleList<TValue> GetValues();
}