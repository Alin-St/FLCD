using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Grammar
{
    public static string EPSILON = "epsilon";

    public List<string> N { get; set; }  // non-terminals
    public List<string> E { get; set; }  // terminals
    public string S { get; set; }        // starting symbol/ axiom
    public Dictionary<string, List<List<string>>> P { get; set; }  // finite set of productions

    public Grammar()
    {
        N = new List<string>();
        E = new List<string>();
        S = "";
        P = new Dictionary<string, List<List<string>>>();
    }

    public void Rebuild()
    {
        N.Clear();
        E.Clear();
        S = "";
        P.Clear();
    }

    static List<string> ProcessLine(string line, string delimiter = " ")
    {
        var elements = new List<string>(line.Trim().Trim('{', '}').Split(delimiter));

        if (elements.Count > 1)
        {
            elements[0] += delimiter;
            var combined = string.Join("", elements.GetRange(0, 2));
            elements.RemoveRange(0, 2);
            elements.Insert(0, combined);
        }

        return elements.ConvertAll(element => element.Trim());
    }

    public void ReadFromFile(string fileName)
    {
        Rebuild();
        using var file = new StreamReader(fileName);
        string line = file.ReadLine()!;
        N = ProcessLine(line.Split('=')[1], ", ");

        line = file.ReadLine()!;
        E = ProcessLine(line[(line.IndexOf('=') + 1)..].Trim(), ", ");

        line = file.ReadLine()!;
        S = ProcessLine(line.Split('=')[1], ", ")[0];

        while ((line = file.ReadLine()!) != null && line.Trim() != "" && !line.Contains(" -> ")) ;

        while (line != null)
        {
            if (line.Contains(" -> "))
            {
                string[] parts = line.Split(" -> ");
                string source = parts[0].Trim();
                string productions = parts[1];
                foreach (var production in productions.Split('|'))
                {
                    var productionArr = production.Trim().Replace("epsilon", EPSILON).Split();
                    if (P.ContainsKey(source))
                    {
                        P[source].Add(productionArr.ToList());
                    }
                    else
                    {
                        P[source] = new List<List<string>> { productionArr.ToList() };
                    }
                }
            }

            line = file.ReadLine()!;
        }
    }

    public bool CheckCfg()
    {
        bool hasStartingSymbol = false;

        foreach (string key in P.Keys)
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

        foreach (List<List<string>> production in P.Values)
        {
            foreach (List<string> rhs in production)
            {
                foreach (string value in rhs)
                {
                    if (!N[0].Split().Contains(value) && !E[0].Split().Contains(value) && value != EPSILON)
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
        string result = "N = " + ListToString(N) + "\n";
        result += "E = " + ListToString(E) + "\n";
        result += "S = " + S + "\n";
        result += "P = " + DictToString(P) + "\n";
        return result;
    }

    static string ListToString(List<string> list)
    {
        return "[" + string.Join(", ", list.Select(x => $"'{x}'")) + "]";
    }

    static string ListOfListsToString(List<List<string>> list)
    {
        return "[" + string.Join(", ", list.Select(x => ListToString(x))) + "]";
    }

    static string DictToString(Dictionary<string, List<List<string>>> dict)
    {
        return "{" + string.Join(", ", dict.Select(p => $"'{p.Key}': {ListOfListsToString(p.Value)}")) + "}";
    }
}
