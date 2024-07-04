namespace Domain;

public interface IDocumentation
{
    NonEmptyString Summary { get; }
    string? Remarks { get; }
}