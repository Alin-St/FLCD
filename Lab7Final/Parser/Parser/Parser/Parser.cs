using System;
using System.IO;
using System.Collections.Generic;

namespace DS
{
    public class Parser
    {
        private Grammar grammar;
        private string outFile;
        private List<string> sequence;
        private List<(string, int)> working;
        private List<string> input;
        private Dictionary<string, int> currentProductionIndices;
        private bool anotherTryPerformed;
        private string state;
        private int index;

        public Parser(Grammar grammar, string sequenceFile, string outFile)
        {
            this.grammar = grammar;
            this.outFile = outFile;
            this.sequence = ReadSequence(sequenceFile);
            this.working = new List<(string, int)>();
            this.input = new List<string> { grammar.GetStartSymbol() };
            this.currentProductionIndices = new Dictionary<string, int>();
            this.anotherTryPerformed = false;
            this.state = "q";
            this.index = 0;
        }

        private List<string> ReadSequence(string seqFile)
        {
            Directory.CreateDirectory("out");
            File.WriteAllText(outFile, string.Empty);

            var terminals = grammar.GetTerminals()[0].Split();
            var terminalIdMapping = new Dictionary<int, string>();
            for (int i = 0; i < terminals.Length; i++)
            {
                terminalIdMapping[i + 1] = terminals[i];
            }

            var seq = new List<string>();
            using (var reader = new StreamReader(seqFile))
            {
                if (seqFile == "sequence/PIF.out")
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var parts = line.Split(" -> ");
                        if (parts.Length == 2)
                        {
                            var tokenId = int.Parse(parts[0].Trim());
                            seq.AddRange(new[] { terminalIdMapping[tokenId] });
                        }
                    }
                }
                else
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        seq.Add(line.Trim());
                    }
                }
            }
            return seq;
        }

        private void GetSituation()
        {
            var msg = $"({state}, {index}, {string.Join(", ", working)}, {string.Join(", ", input)})\n";
            File.AppendAllText(outFile, msg + "\n");
            Console.WriteLine(msg);
        }

        private void Expand()
        {
            var msg = "|--- expand\n";
            Console.WriteLine(msg);
            File.AppendAllText(outFile, msg + "\n");

            var nonTerminal = input[0];
            working.Add((nonTerminal, 0));
            var newProduction = grammar.GetProductionsForNonTerminal(nonTerminal)[0];
            input = newProduction.Concat(input).ToList();
        }

        private void Advance()
        {
            var msg = "|--- advance\n";
            Console.WriteLine(msg);
            File.AppendAllText(outFile, msg + "\n");

            working.Add((input[0], 0));
            input.RemoveAt(0);
            index++;
        }

        private void MomentaryInsuccess()
        {
            var msg = "|--- momentary insuccess\n";
            Console.WriteLine(msg);
            File.AppendAllText(outFile, msg + "\n");

            state = "b";
        }

        private void Back()
        {
            var msg = "|--- back\n";
            Console.WriteLine(msg);
            File.AppendAllText(outFile, msg + "\n");

            var item = working[^1];
            working.RemoveAt(working.Count - 1);
            input.Insert(0, item.Item1);
            index--;
        }

        private void Success()
        {
            var msg = "|--- success\n";
            Console.WriteLine(msg);
            File.AppendAllText(outFile, msg + "\n");

            state = "f";
            msg = $"(f, {index}, {string.Join(", ", working)}, {string.Join(", ", input)})\n=> sequence is syntactically correct\n";
            Console.WriteLine(msg);
            File.AppendAllText(outFile, msg + "\n");
        }

        private void AnotherTry()
        {
            var msg = "|--- another try\n";
            Console.WriteLine(msg);
            File.AppendAllText(outFile, msg + "\n");

            if (working.Count > 0)
            {
                var lastNT = working[^1];
                var (nt, productionNr) = lastNT;

                var productions = grammar.GetProductionsForNonTerminal(nt);

                if (productionNr + 1 < productions.Count)
                {
                    state = "q";

                    var newTuple = (nt, productionNr + 1);
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

        private void Error()
        {
            var msg = "|--- error\n";
            Console.WriteLine(msg);
            File.AppendAllText(outFile, msg + "\n");

            state = "e";
            msg = $"(e, {index}, {string.Join(", ", working)}, {string.Join(", ", input)})\nNo more input to look at!";
            Console.WriteLine(msg);
            File.AppendAllText(outFile, msg + "\n");
        }

        public void Run()
        {
            while (state != "f" && state != "e")
            {
                GetSituation();
                if (state == "q")
                {
                    if (input.Count == 0 && index == sequence.Count)
                    {
                        Success();
                    }
                    else
                    {
                        if (grammar.GetNonTerminals()[0].Split().Contains(input[0]))
                        {
                            Expand();
                        }
                        else
                        {
                            if (index < sequence.Count && input[0] == sequence[index])
                            {
                                Advance();
                            }
                            else
                            {
                                MomentaryInsuccess();
                            }
                        }
                    }
                }
                else
                {
                    if (state == "b")
                    {
                        if (working.Count > 0 && grammar.GetTerminals()[0].Split().Contains(working[^1].Item1))
                        {
                            Back();
                        }
                        else
                        {
                            AnotherTry();
                        }
                    }
                }
            }

            if (state == "e")
            {
                GetSituation();
                Error();
            }
        }
    }
}
