using System;

class Program
{
    static void Main()
    {
        Directory.SetCurrentDirectory("../../../_Run");

        var g = new Grammar();
        string fileName = "g1.txt";
        g.ReadFromFile(fileName);

        Console.WriteLine(g);

        string baseName = fileName.Split('.')[0];
        if (g.CheckCfg())
        {
            Console.WriteLine($"The grammar {baseName} is a CFG\n");
        }
        else
        {
            Console.WriteLine($"The grammar {baseName} is not a CFG\n");
        }

        fileName = "g2.txt";
        g.ReadFromFile(fileName);

        Console.WriteLine(g);

        baseName = fileName.Split('.')[0];
        if (g.CheckCfg())
        {
            Console.WriteLine($"The grammar {baseName} is a CFG\n");
        }
        else
        {
            Console.WriteLine($"The grammar {baseName} is not a CFG\n");
        }

        fileName = "g3.txt";
        g.ReadFromFile(fileName);

        Console.WriteLine(g);

        baseName = fileName.Split('.')[0];
        if (g.CheckCfg())
        {
            Console.WriteLine($"The grammar {baseName} is a CFG\n");
        }
        else
        {
            Console.WriteLine($"The grammar {baseName} is not a CFG\n");
        }
    }
}
