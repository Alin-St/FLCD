﻿namespace Interpreter;

internal class Program
{
    static void Main()
    {
        Directory.SetCurrentDirectory(@"C:\Alin\Projects\UBB-Sem-V\FLCD\FCLD\Interpreter\Interpreter\_Run\");

        var scanner = new Scanner();
        try
        {
            scanner.Scan(File.ReadAllText("sample_program.txt"));
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            return;
        }

        DisplaySymbolTable(scanner.SymbolTable);
        Console.WriteLine();
        DisplayPIF(scanner.PIF);
    }

    static void DisplaySymbolTable(SymbolTable st)
    {
        var symbols = st.ListAll();
        Console.WriteLine("Symbol table:");
        foreach (var (token, position) in symbols)
            Console.WriteLine($"{position} -> {token}");
    }

    static void DisplayPIF(ProgramInternalForm pif)
    {
        Console.WriteLine("PIF:");
        foreach (var entry in pif.Tokens)
            Console.WriteLine($"{entry.Token} -> {entry.STPosition}");
    }
}
