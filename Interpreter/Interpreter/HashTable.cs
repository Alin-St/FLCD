namespace Interpreter;

public class HashTable<TKey, TValue>
{
    private const int InitialCapacity = 0x100000;
    private readonly List<KeyValuePair<TKey, TValue>>[] buckets;

    public int Count { get; private set; }

    public HashTable()
    {
        buckets = new List<KeyValuePair<TKey, TValue>>[InitialCapacity];
    }

    private int GetHashIndex(TKey key)
    {
        return Math.Abs(key!.GetHashCode()) % buckets.Length;
    }

    public void Add(TKey key, TValue value)
    {
        int index = GetHashIndex(key);
        if (buckets[index] == null)
        {
            buckets[index] = new List<KeyValuePair<TKey, TValue>>();
        }

        buckets[index].Add(new KeyValuePair<TKey, TValue>(key, value));
        Count++;
    }

    public TValue Find(TKey key)
    {
        int index = GetHashIndex(key);
        if (buckets[index] != null)
        {
            foreach (var pair in buckets[index])
            {
                if (pair.Key!.Equals(key))
                {
                    return pair.Value;
                }
            }
        }

        throw new KeyNotFoundException("Key not found in the hash table");
    }
}
