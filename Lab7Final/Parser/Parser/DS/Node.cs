public class Node
{
    public int Father { get; set; }
    public int Sibling { get; set; }
    public int Value { get; }
    public int Production { get; set; }

    public Node(int value)
    {
        Father = -1;
        Sibling = -1;
        Value = value;
        Production = -1;
    }

    public override string ToString()
    {
        return $"{Value}  {Father}  {Sibling}";
    }
}
