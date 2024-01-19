namespace Parser;

class Program
{
    static void Main()
    {
        Directory.SetCurrentDirectory("../../../_Run");

        while (true)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Run Parser 1");
            Console.WriteLine("2. Run Parser 2");
            Console.WriteLine("3. Run Parser 3");
            Console.WriteLine("0. Quit");

            string choice = Console.ReadLine()!;

            string sequenceFile, grammarFile;

            switch (choice)
            {
                case "1":
                    sequenceFile = "seq1.txt";
                    grammarFile = "g1.txt";
                    break;
                case "2":
                    sequenceFile = "seq2.txt";
                    grammarFile = "g2.txt";
                    break;
                case "3":
                    sequenceFile = "seq3.txt";
                    grammarFile = "g3.txt";
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please select a valid option.");
                    continue;
            }

            var grammar = new Grammar();
            grammar.ReadFromFile(grammarFile);

            var parser = new Parser(grammar, sequenceFile);
            parser.Run();
        }
    }
}
