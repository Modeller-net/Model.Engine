namespace Model.Domain;

public sealed record Settings
{
    public required string TemplateFolder { get; init; }
    public required string DefinitionFolder { get; init; }
    public required string OutputFolder { get; init; }
    public required string Target { get; init; }
    public string? TemplateName { get; init; }
    public string? DefinitionName { get; init; }
    public bool SupportRegen { get; init; } = true;
    public string Version { get; init; } = "1.0.0";

    public static Settings Default => new()
    {
        TemplateFolder = DefaultTemplateFolder.FullName,
        DefinitionFolder = DefaultDefinitionFolder.FullName,
        OutputFolder = DefaultOutputFolder.FullName,
        Target = Targets.Default
    };

    public static DirectoryInfo DefaultTemplateFolder =>
        DevUtilities.GetSolutionDirectory(Constants.DefaultTemplateFolder);

    public static DirectoryInfo DefaultDefinitionFolder =>
        DevUtilities.GetSolutionDirectory(Constants.DefaultDefinitionFolder);

    public static DirectoryInfo DefaultOutputFolder => new(Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Constants.ToolName,
        Constants.DefaultOutputFolder));

    public static string DefaultFilename { get; set; } = nameof(Settings);
}