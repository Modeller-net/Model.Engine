namespace Modeller.NET.Tool.Core;

public sealed class DefinitionSettings : LogCommandSettings
{
    [CommandArgument(0, "<definition>")]
    [Description("Name of the definition to use.")]
    public string DefinitionName { get; init; } = null!;

    [CommandOption("--definitions")]
    [Description("Folder where the definitions are located.")]
    public string? DefinitionFolder { get; init; } = Settings.DefaultDefinitionFolder.FullName;

    [CommandOption("--target")]
    [Description("Target a specific framework version.")]
    [DefaultValue("net8.0")]
    public string Target { get; init; } = Targets.Default;

    [CommandOption("--output")]
    [Description("Folder where the output is generated.")]
    public string OutputFolder { get; init; } = Settings.DefaultOutputFolder.FullName;
}