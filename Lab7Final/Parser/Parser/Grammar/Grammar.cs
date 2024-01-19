using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DS
{
    public class Grammar
    {
        private List<string> N;
        private List<string> E;
        private string S;
        private Dictionary<string, List<List<string>>> P;

        public Grammar()
        {
            N = new List<string>();
            E = new List<string>();
            S = "";
            P = new Dictionary<string, List<List<string>>>();
        }

        public void Rebuild()
        {
            N = new List<string>();
            E = new List<string>();
            S = "";
            P = new Dictionary<string, List<List<string>>>();
        }

        private static List<string> ProcessLine(string line, string delimiter = " ")
        {
            var elements = line.Trim().Trim('{}').Split(delimiter);
            if (elements.Length > 1)
            {
                elements[0] += delimiter;
                elements[0] = string.Concat(elements[0], elements[1]);
                elements = elements.Skip(1).ToArray();
            }

            return elements.Select(element => element.Trim()).Where(element => !string.IsNullOrEmpty(element)).ToList();
        }

        public void ReadFromFile(string fileName)
        {
            Rebuild();
            using (var file = new StreamReader(fileName))
            {
                var line = file.ReadLine();
                N = ProcessLine(line.Split('=')[1], ", ");

                line = file.ReadLine();
                E = ProcessLine(line.Substring(line.IndexOf('=') + 1, line.Length - 2).Trim(), ", ");

                line = file.ReadLine();
                S = ProcessLine(line.Split('=')[1], ", ")[0];

                while ((line = file.ReadLine()) != null)
                {
                    if (line.Trim() == "" || line.Trim().StartsWith("//"))
                        continue;

                    if (line.Contains("->"))
                    {
                        var parts = line.Split("->");
                        var source = parts[0].Trim();
                        var productions = parts[1].Split('|').Select(prod => ProcessLine(prod)).ToList();

                        if (P.ContainsKey(source))
                            P[source].AddRange(productions);
                        else
                            P[source] = productions;
                    }
                }
            }
        }

        public bool CheckCFG()
        {
            var hasStartingSymbol = false;
            foreach (var key in P.Keys)
            {
                if (key == S)
                    hasStartingSymbol = true;
                if (!N[0].Split().Contains(key))
                    return false;
            }
            if (!hasStartingSymbol)
                return false;
            foreach (var production in P.Values.SelectMany(productions => productions))
            {
                foreach (var value in production)
                {
                    if (!N[0].Split().Contains(value) && !E[0].Split().Contains(value))
                        return false;
                }
            }
            return true;
        }

        public List<string> GetNonTerminals()
        {
            return N;
        }

        public List<string> GetTerminals()
        {
            return E;
        }

        public string GetStartSymbol()
        {
            return S;
        }

        public Dictionary<string, List<List<string>>> GetProductions()
        {
            return P;
        }

        public List<List<string>> GetProductionsForNonTerminal(string nt)
        {
            return P.TryGetValue(nt, out var productions) ? productions : new List<List<string>>();
        }

        public override string ToString()
        {
            var result = $"N = {string.Join(", ", N)}\n";
            result += $"E = {string.Join(", ", E)}\n";
            result += $"S = {S}\n";
            result += $"P = {string.Join(", ", P)}\n";
            return result;
        }
    }
}
