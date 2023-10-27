## LAB 2

SymbolTable is a class that allows you to save tokens (string values). Each token is given an unique index.
When you add a token to the SymbolTable, if the token already exists, it returns its position, otherwise a new
unique position is assigned to it. Possitions are assigned in increasing order.

The SymbolTable uses a custom hash table named HashTable to store its data. HashTable is implemented using a
bucket array of 1M elements. Each bucket is a System.Collections.Generic.List<KeyValuePair<TKey, TValue>>.

GitHub: https://github.com/Alin-St/FCLD
