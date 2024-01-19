using ConsoleTables;
using System.Text;

namespace Parser;

class ParserOutput
{
    private List<Node> Tree { get; }

    public Grammar Grammar { get; }
    public List<string> Sequence { get; }

    public ParserOutput(Grammar grammar, string sequenceFile)
    {
        Grammar = grammar;
        Sequence = ReadSequence(sequenceFile);
        Tree = new();
    }

    static List<string> ReadSequence(string seqFile)
    {
        var seq = new List<string>();
        using (var reader = new StreamReader(seqFile))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
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
            if (working[index] is Tuple<string, int> t)
                Tree.Add(new Node(t.Item1));
            else
                Tree.Add(new Node((string)working[index]));
        }

        for (int index = 0; index < working.Count; index++)
        {
            if (working[index] is Tuple<string, int> t)
            {
                Tree[index].Father = father;
                father = index;
                int lenProd = Grammar.Productions[t.Item1][t.Item2].Count;
                var vectorIndex = new List<int>();

                for (int i = 1; i <= lenProd; i++)
                {
                    vectorIndex.Add(index + i);
                }
                for (int i = 0; i < lenProd; i++)
                {
                    if (Tree[vectorIndex[i]].Production != -1)
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
                    Tree[vectorIndex[i]].Sibling = vectorIndex[i + 1];
                }
            }
            else
            {
                Tree[index].Father = father;
                father = -1;
            }
        }
    }

    private int GetLenDepth(int index, List<object> working)
    {
        var t = (Tuple<string, int>)working[index];
        var production = Grammar.Productions[t.Item1][t.Item2];
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

    public void WriteParsingTree(string state, List<object> working, string? outputFile = null)
    {
        if (state != "e")
        {
            var table = new List<List<object>> { new() { "index", "value", "father", "sibling" } };
            for (int index = 0; index < working.Count; index++)
            {
                table.Add(new List<object> { index, Tree[index].Value, Tree[index].Father, Tree[index].Sibling });
            }

            Console.WriteLine("Parsing tree:");
            Console.WriteLine(MakeTable(table));

            if (outputFile != null)
            {
                using var file = new StreamWriter(outputFile);
                file.WriteLine("Parsing tree:");
                file.WriteLine(MakeTable(table));
            }
        }
    }

    static string MakeTable(List<List<object>> table)
    {
        var ct = new ConsoleTable(table[0].Select(x => $"{x}").ToArray());
        foreach (var row in table.Skip(1))
        {
            ct.AddRow(row.Select(x => $"{x}").ToArray());
        }
        return ct.ToString();
    }
}