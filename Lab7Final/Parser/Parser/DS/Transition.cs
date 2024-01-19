public class Transition
{
    public int FromState { get; private set; }
    public int ToState { get; private set; }
    public string Label { get; private set; }

    public Transition(int fromState, int toState, string label)
    {
        FromState = fromState;
        ToState = toState;
        Label = label;
    }

    public int GetFrom()
    {
        return FromState;
    }

    public int GetTo()
    {
        return ToState;
    }

    public string GetLabel()
    {
        return Label;
    }

    public void SetFrom(int fromState)
    {
        FromState = fromState;
    }

    public void SetTo(int toState)
    {
        ToState = toState;
    }

    public void SetLabel(string label)
    {
        Label = label;
    }
}
