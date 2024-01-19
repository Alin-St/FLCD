namespace Interpreter;

internal class FiniteAutomaton
{
    public HashSet<string> States { get; private set; } = new();
    public HashSet<char> Alphabet { get; private set; } = new();
    public Dictionary<(string state, char symbol), string> Transitions { get; private set; } = new();
    public string InitialState { get; private set; }
    public HashSet<string> FinalStates { get; private set; } = new();

    public FiniteAutomaton(string filename)
    {
        foreach (var line  in File.ReadAllLines(filename))
        {
            var words = line.Split(' ');
            if (words.Length < 2)
                throw new Exception($"Invalid FA line: {line}");
            if ((words[0] == "transition" && words.Length != 4) || (words[0] != "transition" && words.Length != 2))
                throw new Exception($"Invalid FA line: {line}");

            var elementType = words[0];
            var elementValue = words[1];

            switch (elementType)
            {
                case "state":
                    States.Add(elementValue);
                    break;

                case "alphabet":
                    Alphabet.Add(elementValue[0]);
                    break;

                case "transition":
                    Transitions.Add((words[1], words[2][0]), words[3]);
                    break;

                case "initial":
                    InitialState = elementValue;
                    break;

                case "final":
                    FinalStates.Add(elementValue);
                    break;

                default:
                    throw new Exception($"Invalid FA element type: {elementType}");
            }
        }

        if (InitialState == null)
            throw new Exception("No initial state found.");
    }

    // Check if a sequence is accepted by the FA
    public bool IsAccepted(string sequence)
    {
        string currentState = InitialState;

        foreach (char symbol in sequence)
        {
            if (!Alphabet.Contains(symbol))
                return false;

            if (!Transitions.ContainsKey((currentState, symbol)))
                return false;

            currentState = Transitions[(currentState, symbol)];
        }

        return FinalStates.Contains(currentState);
    }
}
