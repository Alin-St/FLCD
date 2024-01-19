using DS;
using System;
using System.Collections.Generic;
using System.IO;

namespace Parser.Parser;

public class ParserOutput
{
    private Grammar grammar;
    private string outFile;
    private List<string> sequence;
    private List<Node> tree;

    public ParserOutput(Grammar grammar, string sequenceFile, string outFile)
    {
        this.grammar = grammar;
        this.outFile = outFile;
        sequence = ReadSequence(sequenceFile);
        tree = new List<Node>();
    }

    private List<string> ReadSequence(string seqFile)
    {
        var seq = new List<string>();
        using (var reader = new StreamReader(seqFile))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                seq.Add(line.Trim());
            }
        }
        return seq;
    }

    public void CreateParsingTree(List<object> working)
    {
        int father = -1;
        for (int index = 0; index < working.Count; index++)
        {
            if (working[index] is Tuple<string, int>)
            {
                tree.Add(new Node(((Tuple<string, int>)working[index]).Item1));
                tree[index].Production = ((Tuple<string, int>)working[index]).Item2;
            }
            else
            {
                tree.Add(new Node((string)working[index]));
            }
        }

        for (int index = 0; index < working.Count; index++)
        {
            if (working[index] is Tuple<string, int>)
            {
                tree[index].Father = father;
                father = index;
                int lenProd = grammar.GetProductions()[((Tuple<string, int>)working[index]).Item1][((Tuple<string, int>)working[index]).Item2].Count;
                List<int> vectorIndex = new List<int>();
                for (int i = 1; i <= lenProd; i++)
                {
                    vectorIndex.Add(index + i);
                }
                for (int i = 0; i < lenProd; i++)
                {
                    if (tree[vectorIndex[i]].Production != -1)
                    {
                        int offset = GetLenDepth(vectorIndex[i], working);
                        for (int j = i + 1; j < lenProd; j++)
                        {
                            vectorIndex[j] += offset;
                        }
                    }
                }
                for (int i = 0; i < lenProd - 1; i++)
                {
                    tree[vectorIndex[i]].Sibling = vectorIndex[i + 1];
                }
            }
            else
            {
                tree[index].Father = father;
                father = -1;
            }
        }
    }

    private int GetLenDepth(int index, List<object> working)
    {
        var production = grammar.GetProductions()[((Tuple<string, int>)working[index]).Item1][((Tuple<string, int>)working[index]).Item2];
        int lenProd = production.Count;
        int sum = lenProd;
        for (int i = 1; i <= lenProd; i++)
        {
            if (working[index + i] is Tuple<string, int>)
            {
                sum += GetLenDepth(index + i, working);
            }
        }
        return sum;
    }

    public void WriteParsingTree(string state, List<object> working)
    {
        if (state != "e")
        {
            var table = new List<List<string>> { new List<string> { "index", "value", "father", "sibling" } };
            for (int index = 0; index < working.Count; index++)
            {
                table.Add(new List<string>
                {
                    index.ToString(),
                    tree[index].Value,
                    tree[index].Father.ToString(),
                    tree[index].Sibling.ToString()
                });
            }

            Console.WriteLine("Parsing tree:");
            Console.WriteLine(tabulate(table, "firstrow", "grid"));

            using (var writer = new StreamWriter(outFile, true))
            {
                writer.WriteLine("\nParsing tree:");
                writer.WriteLine(tabulate(table, "firstrow", "grid"));
            }
        }
    }

    private string tabulate(List<List<string>> table, string headers, string tableFormat)
    {
        throw new NotImplementedException("Implement tabulate function based on your requirements.");
        // The tabulate function is not directly available in C#.
        // You may need to implement a similar formatting logic based on your requirements.
        // You can consider using StringBuilder or other string manipulation methods for formatting.
    }
}
