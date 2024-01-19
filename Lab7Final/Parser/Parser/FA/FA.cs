using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DS
{
    public class FA
    {
        private string filename;
        private List<string> states;
        private List<string> alphabet;
        private List<Transition> transitions;
        private string initialState;
        private List<string> outputStates;

        public FA(string filename)
        {
            this.filename = filename;
            this.states = new List<string>();
            this.alphabet = new List<string>();
            this.transitions = new List<Transition>();
            this.initialState = "";
            this.outputStates = new List<string>();
            Init();
        }

        private void Init()
        {
            Regex regex = new Regex("^([a-z_]*)=");
            using (StreamReader file = new StreamReader(this.filename))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    Match match = regex.Match(line);
                    if (match.Success)
                    {
                        string keyword = match.Groups[1].Value;
                        if (keyword == "states")
                        {
                            string statesWithCurlyBrackets = line.Substring(line.IndexOf("=") + 1);
                            string states = statesWithCurlyBrackets[1..^2].Trim();
                            this.states = new List<string>(states.Split(','));
                        }
                        else if (keyword == "alphabet")
                        {
                            string alphabetWithCurlyBrackets = line.Substring(line.IndexOf("=") + 1);
                            string alphabet = alphabetWithCurlyBrackets[1..^2].Trim();
                            this.alphabet = new List<string>(alphabet.Split(','));
                        }
                        else if (keyword == "out_states")
                        {
                            string outputStatesWithCurlyBrackets = line.Substring(line.IndexOf("=") + 1);
                            string outputStates = outputStatesWithCurlyBrackets[1..^2].Trim();
                            this.outputStates = new List<string>(outputStates.Split(','));
                        }
                        else if (keyword == "initial_state")
                        {
                            this.initialState = line.Substring(line.IndexOf("=") + 1).Trim();
                        }
                        else if (keyword == "transitions")
                        {
                            string transitionsWithCurlyBrackets = line.Substring(line.IndexOf("=") + 1);
                            string transitions = transitionsWithCurlyBrackets[1..^1].Trim();
                            string[] transitionsList = transitions.Split(';');
                            foreach (string transition in transitionsList)
                            {
                                string transitionWithoutParentheses = transition[1..^1].Trim();
                                string[] individualValues = transitionWithoutParentheses.Split(',');
                                this.transitions.Add(new Transition(individualValues[0], individualValues[1], individualValues[2]));
                            }
                        }
                        else
                        {
                            throw new Exception($"Unexpected keyword '{keyword}' in file");
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid line: " + line);
                    }
                }
            }
        }

        public void PrintStates()
        {
            PrintListOfString("states", this.states);
        }

        public void PrintAlphabet()
        {
            PrintListOfString("alphabet", this.alphabet);
        }

        public void PrintOutputStates()
        {
            PrintListOfString("out_states", this.outputStates);
        }

        public void PrintInitialState()
        {
            Console.WriteLine($"initial_state = {this.initialState}");
        }

        public void PrintTransitions()
        {
            Console.Write("transitions = {");
            for (int i = 0; i < this.transitions.Count; i++)
            {
                Transition transition = this.transitions[i];
                if (i != this.transitions.Count - 1)
                {
                    Console.Write($"({transition.GetFrom()}, {transition.GetTo()}, {transition.GetLabel()}); ");
                }
                else
                {
                    Console.Write($"({transition.GetFrom()}, {transition.GetTo()}, {transition.GetLabel()})");
                }
            }
            Console.WriteLine("}");
        }

        public string GetNextAccepted(string word)
        {
            string current_state = this.initialState;
            string accepted_word = "";
            foreach (char c in word)
            {
                string new_state = null;
                foreach (Transition transition in this.transitions)
                {
                    if (transition.GetFrom() == current_state && transition.GetLabel() == c.ToString())
                    {
                        new_state = transition.GetTo();
                        accepted_word += c;
                        break;
                    }
                }
                if (new_state == null)
                {
                    if (!this.outputStates.Contains(current_state))
                    {
                        return null;
                    }
                    else
                    {
                        return accepted_word;
                    }
                }
                current_state = new_state;
            }
            return accepted_word;
        }

        private static void PrintListOfString(string listName, List<string> list)
        {
            Console.WriteLine($"{listName} = {{{string.Join(", ", list)}}}");
        }
    }
}
