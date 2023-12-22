class Program
{
    static void Main()
    {
        Directory.SetCurrentDirectory("../../../_Run");

        Grammar g = new();
        string fileName = "g1.txt";
        g.ReadFromFile(fileName);

        Console.WriteLine(g.ToString());

        if (g.CheckCFG())
        {
            Console.WriteLine($"The grammar {fileName} is a CFG\n");
        }
        else
        {
            Console.WriteLine($"The grammar {fileName} is not a CFG\n");
        }

        fileName = "g2.txt";
        g.ReadFromFile(fileName);

        Console.WriteLine(g.ToString());

        if (g.CheckCFG())
        {
            Console.WriteLine($"The grammar {fileName} is a CFG\n");
        }
        else
        {
            Console.WriteLine($"The grammar {fileName} is not a CFG\n");
        }
    }
}
