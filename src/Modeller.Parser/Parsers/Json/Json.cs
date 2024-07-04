namespace Modeller.Parsers;

public abstract class Json
{
}

public class JsonArray(ImmutableArray<Json> elements) : Json
{
    public ImmutableArray<Json> Elements { get; } = elements;

    public override string ToString()
        => $"[{string.Join(",", Elements.Select(e => e.ToString()))}]";
}

public class JsonObject(IImmutableDictionary<string, Json> members) : Json
{
    public IImmutableDictionary<string, Json> Members { get; } = members;

    public override string ToString()
        => $"{{{string.Join(",", Members.Select(kvp => $"\"{kvp.Key}\":{kvp.Value}"))}}}";
}

public class JsonString(string value) : Json
{
    public string Value { get; } = value;

    public override string ToString()
        => $"\"{Value}\"";
}
