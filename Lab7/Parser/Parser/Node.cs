namespace Parser;

class Node
{
    public int Father { get; set; } = -1;
    public int Sibling { get; set; } = -1;
    public string Value { get; set; }
    public int Production { get; set; } = -1;

    public Node(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return $"{Value}  {Father}  {Sibling}";
    }
}
