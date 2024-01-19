namespace Parser;

class Parser
{
    private readonly Grammar grammar;
    private readonly List<string> sequence;
    private readonly List<object> working;
    private List<object> input;
    private string state;
    private int index;

    public Parser(Grammar grammar, string sequenceFile)
    {
        this.grammar = grammar;
        this.sequence = ReadSequence(sequenceFile);
        this.working = new List<object>();
        this.input = new List<object> { this.grammar.StartSymbol };
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
        Console.WriteLine($"({this.state}, {this.index}, {string.Join(", ", this.working)}, {string.Join(", ", this.input)})");
    }

    public void Expand()
    {
        Console.WriteLine("|--- expand");
        var nonTerminal = (string)this.input.First();
        this.input.RemoveAt(0);
        this.working.Add(new Tuple<string, int>(nonTerminal, 0));

        var newProduction = this.grammar.GetProductionsForNonTerminal(nonTerminal)[0];
        this.input = newProduction.Concat(this.input).ToList();
    }

    public void Advance()
    {
        Console.WriteLine("|--- advance");
        var item = this.input.First();
        this.input.RemoveAt(0);
        this.working.Add(item);
        this.index++;
    }

    public void MomentaryInsuccess()
    {
        Console.WriteLine("|--- momentary insuccess");
        this.state = "b";
    }

    public void Back()
    {
        Console.WriteLine("|--- back");
        var item = this.working.Last();
        this.working.RemoveAt(this.working.Count - 1);
        this.input.Insert(0, item);
        this.index--;
    }

    public void Success()
    {
        Console.WriteLine("|--- success");
        this.state = "f";
        var msg = $"(f, {this.index}, {string.Join(", ", this.working)}, {string.Join(", ", this.input)})\n=> sequence is syntactically correct\n";
        Console.WriteLine(msg);
    }

    public void AnotherTry()
    {
        Console.WriteLine("|--- another try");
        if (this.working.Any())
        {
            var lastNt = (Tuple<string, int>)this.working.Last();
            var nt = lastNt.Item1;
            var productionNr = lastNt.Item2;

            var productions = this.grammar.GetProductionsForNonTerminal(nt);

            if (productionNr + 1 < productions.Count)
            {
                this.state = "q";

                var newTuple = Tuple.Create(nt, productionNr + 1);
                this.working.Add(newTuple);

                var lenLastProduction = productions[productionNr].Count;
                this.input = this.input.Skip(lenLastProduction).ToList();
                var newProduction = productions[productionNr + 1];
                this.input = newProduction.Concat(this.input).ToList();
            }
            else
            {
                var lenLastProduction = productions[productionNr].Count;
                this.input = this.input.Skip(lenLastProduction).ToList();
                if (this.input.Count != 0)
                {
                    this.input.Insert(0, nt);
                }
            }
        }
        else
        {
            this.state = "e";
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
                    if (this.working.Any() && this.grammar.Terminals[0].Split(" ").Contains(((Tuple<string, int>)this.working.Last()).Item1))
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
