namespace Interpreter;

internal class SymbolTable
{
    readonly HashTable<string, int> tokens = new();

    /// <summary> Adds the token to the ST and returns its position.
    /// If the token is already in the ST it only returns the position. </summary>
    public int Add(string token)
    {
        try
        {
            int p = tokens.Find(token);
            return p;
        }
        catch { }

        int position = tokens.Count;
        tokens.Add(token, position);
        return position;
    }
}
