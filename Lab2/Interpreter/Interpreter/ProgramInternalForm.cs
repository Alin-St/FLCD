namespace Interpreter;

internal class ProgramInternalForm
{
    public List<PifEntry> Tokens { get; } = new();

    public void Add(string token, int stPosition)
    {
        Tokens.Add(new(token, stPosition));
    }
}
