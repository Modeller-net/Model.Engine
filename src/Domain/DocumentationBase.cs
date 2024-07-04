namespace Domain;

public abstract record DocumentationBase(NonEmptyString Summary, string? Remarks = null) : IDocumentation;