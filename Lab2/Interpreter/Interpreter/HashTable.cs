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

    /// <summary> Adds the key and value to the hash table. If the key already exists, a duplicate is added. </summary>
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

    /// <summary>  Returns the value associated with the key. If the key is not found, throws an exception. </summary>
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

    public List<KeyValuePair<TKey, TValue>> ListAll()
    {
        var result = new List<KeyValuePair<TKey, TValue>>();

        foreach (List<KeyValuePair<TKey, TValue>> bucket in buckets)
        {
            if (bucket != null)
            {
                foreach (var pair in bucket)
                    result.Add(pair);
            }
        }

        return result;
    }
}
