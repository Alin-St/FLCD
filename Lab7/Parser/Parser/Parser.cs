namespace Parser;

class Parser
{
    private readonly Grammar grammar;
    private readonly List<string> sequence;
    public readonly List<object> working;
    private List<string> input;
    public string state;
    private int index;

    public Parser(Grammar grammar, string sequenceFile)
    {
        this.grammar = grammar;
        this.sequence = ReadSequence(sequenceFile);
        this.working = new List<object>();
        this.input = new List<string> { this.grammar.StartSymbol };
        this.state = "q";
        this.index = 0;
    }

    private static List<string> ReadSequence(string seqFile)
    {
        var seq = new List<string>();
        using (var reader = new StreamReader(seqFile))
        {
            string line;
            while ((line = reader.ReadLine()!) != null)
            {
                seq.Add(line.Trim());
            }
        }
        return seq;
    }

    public void GetSituation()
    {
        Console.WriteLine($"({state}, {index}, {Util.ObjListToString(working)}, {Util.StrListToString(input)})");
    }

    public void Expand()
    {
        Console.WriteLine("|--- expand");
        var nonTerminal = input.First(); // non_terminal = "S"
        input.RemoveAt(0);
        working.Add(Tuple.Create(nonTerminal, 0)); // working = [("S", 0)]

        var newProduction = grammar.GetProductionsForNonTerminal(nonTerminal)[0]; // new_production = ["a", "S", "b", "S"]
        this.input = newProduction.Concat(this.input).ToList();
    }

    public void Advance()
    {
        Console.WriteLine("|--- advance");
        var item = this.input.First();
        this.input.RemoveAt(0);
        this.working.Add(item); // working = [("S", 0), "a"]
        this.index++; // (q, i + 1, w, s)
    }

    public void MomentaryInsuccess()
    {
        Console.WriteLine("|--- momentary insuccess");
        this.state = "b"; // (b, i, w, s)
    }

    public void Back()
    {
        Console.WriteLine("|--- back");
        var item = this.working.Last(); // item = "a"
        this.working.RemoveAt(this.working.Count - 1);
        this.input.Insert(0, (string)item); // self.input = ["S", "a", "S", "b", "S"]
        this.index--;
    }

    public void Success()
    {
        Console.WriteLine("|--- success");
        this.state = "f";
        var msg = $"(f, {this.index}, {Util.ObjListToString(working)}, {Util.StrListToString(input)})\n=> sequence is syntactically correct\n";
        Console.WriteLine(msg);
    }

    public void AnotherTry()
    {
        Console.WriteLine("|--- another try");
        if (working.Any())
        {
            var lastNt = (Tuple<string, int>)this.working.Last();
            working.RemoveAt(working.Count - 1);
            var nt = lastNt.Item1;
            var productionNr = lastNt.Item2;

            var productions = grammar.GetProductionsForNonTerminal(nt);

            if (productionNr + 1 < productions.Count)
            {
                state = "q";

                var newTuple = Tuple.Create(nt, productionNr + 1);
                working.Add(newTuple);

                var lenLastProduction = productions[productionNr].Count;
                input = input.Skip(lenLastProduction).ToList();
                var newProduction = productions[productionNr + 1];
                input = newProduction.Concat(input).ToList();
            }
            else
            {
                var lenLastProduction = productions[productionNr].Count;
                input = input.Skip(lenLastProduction).ToList();
                if (input.Count != 0)
                {
                    input.Insert(0, nt);
                }
            }
        }
        else
        {
            state = "e";
        }
    }

    public void Error()
    {
        Console.WriteLine("|--- error");
        this.state = "e";
        var msg = $"(e, {this.index}, {string.Join(", ", this.working)}, {string.Join(", ", this.input)})\nNo more input to look at!";
        Console.WriteLine(msg);
    }

    public void Run()
    {
        while (this.state != "f" && this.state != "e")
        {
            this.GetSituation();
            if (this.state == "q")
            {
                if (this.input.Count == 0 && this.index == this.sequence.Count)
                {
                    this.Success();
                }
                else
                {
                    if (this.input.First() is string first && this.grammar.NonTerminals[0].Split(" ").Contains(first))
                    {
                        this.Expand();
                    }
                    else
                    {
                        if (this.index < this.sequence.Count && this.input.First().Equals(this.sequence[this.index]))
                        {
                            this.Advance();
                        }
                        else
                        {
                            this.MomentaryInsuccess();
                        }
                    }
                }
            }
            else
            {
                if (this.state == "b")
                {
                    if (this.working.Any() && this.grammar.Terminals[0].Split(" ").Contains(working.Last()))
                    {
                        this.Back();
                    }
                    else
                    {
                        this.AnotherTry();
                    }
                }
            }
        }

        if (this.state == "e")
        {
            this.GetSituation();
            this.Error();
        }
    }
}
