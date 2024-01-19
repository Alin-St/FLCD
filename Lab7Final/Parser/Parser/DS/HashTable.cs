using System;
using System.Collections.Generic;

class HashTable
{
    private List<List<Tuple<string, int>>> hashTable;
    private int capacity;
    private int next;

    public HashTable(int capacity)
    {
        this.capacity = capacity;
        this.hashTable = new List<List<Tuple<string, int>>>(capacity);
        for (int i = 0; i < capacity; i++)
        {
            this.hashTable.Add(new List<Tuple<string, int>>());
        }
        this.next = 0;
    }

    public int GetCapacity()
    {
        return this.capacity;
    }

    public int Hash(object key)
    {
        if (key is int)
        {
            return (int)key % this.capacity;
        }
        else if (key is string)
        {
            int hashValue = 5381;
            foreach (char c in (string)key)
            {
                hashValue = ((hashValue << 5) + hashValue) + (int)c;
            }
            return Math.Abs(hashValue) % this.capacity;
        }
        throw new ArgumentException("Unsupported key type");
    }

    public bool Contains(object key)
    {
        int hashValue = this.GetHashValue(key);
        foreach (Tuple<string, int> item in this.hashTable[hashValue])
        {
            if (item.Item1 == (string)key)
            {
                return true;
            }
        }
        return false;
    }

    private int GetHashValue(object key)
    {
        int hashValue = -1;
        if (key is int || key is string)
        {
            hashValue = this.Hash(key);
        }
        return hashValue;
    }

    public Tuple<int, int> Add(object key)
    {
        int hashValue = this.GetHashValue(key);
        this.hashTable[hashValue].Add(Tuple.Create((string)key, this.next));
        int currentNext = this.next;
        this.next++;
        return Tuple.Create(hashValue, currentNext);
    }

    public Tuple<int, int> GetPosition(object key)
    {
        int hashValue = this.GetHashValue(key);
        if (hashValue != -1 && this.hashTable[hashValue].Count > 0)
        {
            return Tuple.Create(hashValue, this.hashTable[hashValue][0].Item2);
        }
        return Tuple.Create(-1, -1);
    }

    public override string ToString()
    {
        return this.hashTable.ToString();
    }

    public List<List<Tuple<string, int>>> ToList()
    {
        return this.hashTable;
    }
}
