namespace Domain;

public record Field(NameType Name, DataTypeType DataType, NonEmptyString Summary) : DocumentationBase(Summary)
{
    public bool Nullable { get; init; }
    public bool BusinessKey { get; init; }
    public string? Default { get; init; }
    public ValueGeneratedTypes Generated { get; init; } = ValueGeneratedTypes.None;
    public (int start, int increment)? Sequence { get; init; }
    public string? Help { get; init; }
    public string? Label { get; init; }
    public string? Placeholder { get; init; }
    public string? Category { get; init; }
}