namespace Parser;

static class Util
{
    public static string StrListToString(List<string> list)
    {
        return "[" + string.Join(", ", list.Select(x => $"'{x}'")) + "]";
    }

    public static string ListOfStrListsToString(List<List<string>> list)
    {
        return "[" + string.Join(", ", list.Select(x => StrListToString(x))) + "]";
    }

    public static string DictStrListListStrToString(Dictionary<string, List<List<string>>> dict)
    {
        return "{" + string.Join(", ", dict.Select(p => $"'{p.Key}': {ListOfStrListsToString(p.Value)}")) + "}";
    }

    public static string ObjListToString(List<object> list)
    {
        return "[" + string.Join(", ", list.Select(x =>
        {
            if (x is Tuple<string, int> t)
                return $"('{t.Item1}', {t.Item2})";
            return $"'{x}'";
        })) + "]";
    }
}
