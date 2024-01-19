using System;

namespace DS
{
    public class SymbolTable
    {
        private int size;
        private HashTable hashTable;

        public SymbolTable(int size)
        {
            this.size = size;
            this.hashTable = new HashTable(size);
        }

        public Tuple<int, int> AddHash(string name)
        {
            return this.hashTable.Add(name);
        }

        public bool HasHash(string name)
        {
            return this.hashTable.Contains(name);
        }

        public Tuple<int, int> GetPositionHash(string name)
        {
            return this.hashTable.GetPosition(name);
        }

        public override string ToString()
        {
            return $"SymbolTable {{\n{string.Join("\n", this.hashTable.ToList())}\n}}";
        }
    }
}
