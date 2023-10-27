namespace Interpreter;

internal class Program
{
    static void Main()
    {
        SymbolTable symbolTable = new();
        int p1 = symbolTable.Add("Hello");
        int p2 = symbolTable.Add("World");
        int p3 = symbolTable.Add("Hello");

        Console.WriteLine("p1 = {0}, p2 = {1}, p3 = {2}", p1, p2, p3);
    }
}
