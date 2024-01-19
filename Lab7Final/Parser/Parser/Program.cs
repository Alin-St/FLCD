using DS;
using Parser.Parser;
using Parser.Scanner;
using System;
using System.IO;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Using Sequence w = aacbc");
            Console.WriteLine("2. Using program p1.txt, p2.txt, or p3.txt");
            Console.WriteLine("0. Quit");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                string grammarFile = "grammars/g1.in";
                string sequenceFile = "sequence/seq1.txt";
                string outputFile = "out/out1.txt";
                ExecuteParser(grammarFile, sequenceFile, outputFile);
            }
            else if (choice == "2")
            {
                try
                {
                    while (true)
                    {
                        Console.WriteLine("Choose a program:");
                        Console.WriteLine("1. p1.txt");
                        Console.WriteLine("2. p2.txt");
                        Console.WriteLine("3. p3.txt");
                        Console.WriteLine("0. Go back");
                        string programChoice = Console.ReadLine();

                        string scannerProgram;
                        string outputFilePrefix;

                        if (programChoice == "1")
                        {
                            scannerProgram = "p1.txt";
                            outputFilePrefix = "out2p1";
                        }
                        else if (programChoice == "2")
                        {
                            scannerProgram = "p2.txt";
                            outputFilePrefix = "out2p2";
                        }
                        else if (programChoice == "3")
                        {
                            scannerProgram = "p3.txt";
                            outputFilePrefix = "out2p3";
                        }
                        else if (programChoice == "0")
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid program choice. Please select a valid option.");
                            continue;
                        }

                        ExecuteScanner(scannerProgram);

                        string sequenceFile = "sequence/PIF.out";
                        if (File.Exists(sequenceFile))
                        {
                            string grammarFile = "grammars/g2.in";
                            string outputFile = $"out/{outputFilePrefix}.txt";
                            ExecuteParser(grammarFile, sequenceFile, outputFile);
                        }
                        else
                        {
                            Console.WriteLine("PIF.out file does not exist. Please check your scanner.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else if (choice == "0")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid choice. Please select a valid option.");
                continue;
            }
        }
    }

    static void ExecuteScanner(string program)
    {
        Scanner scanner = new Scanner();
        scanner.ReadTokens();
        scanner.Scan(program, "sequence");
    }

    static void ExecuteParser(string grammarFile, string sequenceFile, string outputFile)
    {
        Grammar grammar = new Grammar();
        grammar.ReadFromFile(grammarFile);

        Parser parser = new Parser(grammar, sequenceFile, outputFile);
        parser.Run();

        ParserOutput parserOutput = new ParserOutput(grammar, sequenceFile, outputFile);
        parserOutput.CreateParsingTree(parser.Working);
        parserOutput.WriteParsingTree(parser.State, parser.Working);
    }
}
