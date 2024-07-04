namespace Domain;

public record Page(NameType Name)
{
    public string? Title { get; init; }

    public string? SubTitle { get; init; }

    public string? Description { get; init; }

    public string? Icon { get; init; }

    public IEnumerable<PageFieldContextData> Fields { get; init; } = [];
}