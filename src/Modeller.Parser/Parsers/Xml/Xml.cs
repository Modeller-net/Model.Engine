namespace Modeller.Parsers;

public class Tag(string name, IEnumerable<Attribute> attributes, IEnumerable<Tag>? content)
    : IEquatable<Tag>
{
    public string Name { get; } = name;

    public IEnumerable<Attribute> Attributes { get; } = attributes;

    public IEnumerable<Tag>? Content { get; } = content;

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return Equals((Tag)obj);
    }

    public override int GetHashCode() => HashCode.Combine(Name, Attributes, Content);

    public bool Equals(Tag? other)
        => Name == other?.Name
           && Attributes.SequenceEqual(other.Attributes)
           && ((Content is null && other.Content is null) || Content!.SequenceEqual(other.Content!));
}

[SuppressMessage(
    "naming",
    "CA1711:Rename type name so that it does not end in 'Stream'",
    Justification = "Example code"
)]
public class Attribute(string name, string value) : IEquatable<Attribute>
{
    public string Name { get; } = name;

    public string Value { get; } = value;

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return Equals((Attribute)obj);
    }

    public bool Equals(Attribute? other)
        => Name == other?.Name
           && Value == other.Value;

    public override int GetHashCode() => HashCode.Combine(Name, Value);
}