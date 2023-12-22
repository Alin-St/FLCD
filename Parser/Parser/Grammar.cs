class Grammar
{
    public List<string> N { get; private set; } = new(); // non-terminals
    public List<string> E { get; private set; } = new(); // terminals
    public string S { get; private set; } = ""; // starting symbol/axiom
    public Dictionary<string, List<List<string>>> P { get; private set; } = new(); // finite set of productions

    private static List<string> ProcessLine(string line, string delimiter = " ")
    {
        return line.Trim().TrimStart('{').TrimEnd('}').Split(delimiter)
                   .Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
    }

    private static List<List<string>> ParseProductions(string productions)
    {
        return productions.Split('|')
                          .Select(production => production.Trim().Split().ToList())
                          .ToList();
    }

    public void ReadFromFile(string fileName)
    {
        N.Clear();
        E.Clear();
        S = "";
        P.Clear();

        using var file = new StreamReader(fileName);
        string line;

        while ((line = file.ReadLine()!) != null)
        {
            if (line.StartsWith("N = "))
            {
                N = ProcessLine(line.Split('=')[1], ", ");
            }
            else if (line.StartsWith("E = "))
            {
                E = ProcessLine(line[(line.IndexOf('=') + 1)..].Trim(), ", ");

                // E = ProcessLine(line.Split('=')[1], ", "); // bugged
            }
            else if (line.StartsWith("S = "))
            {
                S = ProcessLine(line.Split('=')[1], ", ")[0];
            }
            else if (line.Contains(" -> "))
            {
                var splitLine = line.Split(new[] { " -> " }, StringSplitOptions.None);
                string source = splitLine[0].Trim();
                string productions = splitLine[1];
                if (P.ContainsKey(source))
                {
                    P[source].AddRange(ParseProductions(productions));
                }
                else
                {
                    P[source] = ParseProductions(productions);
                }
            }
        }
    }

    public bool CheckCFG()
    {
        bool hasStartingSymbol = false;

        foreach (var key in P.Keys)
        {
            if (key == S)
            {
                hasStartingSymbol = true;
            }

            if (!N[0].Split().Contains(key))
            {
                return false;
            }
        }

        if (!hasStartingSymbol)
        {
            return false;
        }

        foreach (var production in P.Values)
        {
            foreach (var rhs in production)
            {
                foreach (var value in rhs)
                {
                    if (!N[0].Split().Contains(value) && !E[0].Split().Contains(value) && value != "ε")
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public override string ToString()
    {
        string result = "N = " + string.Join(", ", N) + "\n";
        result += "E = " + string.Join(", ", E) + "\n";
        result += "S = " + S + "\n";
        result += "P = ";
        foreach (var kv in P)
        {
            result += kv.Key + " -> ";
            result += string.Join(" | ", kv.Value.Select(v => string.Join(" ", v)));
            result += "\n    ";
        }

        return result;
    }
}
