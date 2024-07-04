namespace Domain;

public record PageFieldContextData(NameType Name, int ZOrder)
{
    public string? GroupName { get; init; }
}