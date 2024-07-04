namespace Modeller.NET.Tool.Core;

public sealed class WatchSettings : LogCommandSettings
{
    [CommandArgument(0, "<template>")]
    [Description("Name of the template to use.")]
    public string TemplateName { get; init; } = null!;

    [CommandArgument(1, "<definition>")]
    [Description("Name of the definition to use.")]
    public string DefinitionName { get; init; } = null!;

    [CommandOption("--templates")]
    [Description("Folder where the templates are located.")]
    public string? TemplateFolder { get; init; } = Settings.DefaultTemplateFolder.FullName;

    [CommandOption("--definitions")]
    [Description("Folder where the definitions are located.")]
    public string? DefinitionFolder { get; init; } = Settings.DefaultDefinitionFolder.FullName;

    [CommandOption("--output")]
    [Description("Folder where the output is generated.")]
    public string? OutputFolder { get; init; } = Settings.DefaultOutputFolder.FullName;

    [CommandOption("--regen")]
    [Description("Allow existing files to be overwritten.")]
    [DefaultValue(true)]
    public bool SupportRegen { get; init; } = true;

    [CommandOption("--target")]
    [Description("Target a specific framework version.")]
    [DefaultValue("net8.0")]
    public string Target { get; init; } = Targets.Default;

    [CommandOption("--version")]
    [Description("Specific version to use for the generator")]
    [DefaultValue("1.0.0")]
    public string Version { get; init; } = "1.0.0";
}