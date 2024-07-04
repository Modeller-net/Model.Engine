namespace Modeller.NET.Tool.Commands;

internal class ValidateCommand(
    IAnsiConsole console,
    ILoader<IDefinitionItem> definitionLoader,
    ILogger<ValidateCommand> logger) : Command<ValidateCommand.ValidateSettings>
{
    private readonly IAnsiConsole _console = console ?? throw new ArgumentNullException(nameof(console));
    private readonly ILogger<ValidateCommand> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ILoader<IDefinitionItem> _definitionLoader = definitionLoader ?? throw new ArgumentNullException(nameof(definitionLoader));

    public sealed class ValidateSettings : LogCommandSettings
    {
        [CommandArgument(0, "<definition>")]
        [Description("Name of the definition to use.")]
        public string DefinitionName { get; init; } = null!;

        [CommandOption("--definitions")]
        [Description("Folder where the definitions are located.")]
        public string? DefinitionFolder { get; init; } = Settings.DefaultDefinitionFolder.FullName;
    }

    public override int Execute(CommandContext context, ValidateSettings settings)
    {
        _logger.LogDebug("Validate Command - OnExecute");
        _logger.LogInformation("Definition: {Definition}", $"{settings.DefinitionFolder}/{settings.DefinitionName}");

        _console.MarkupLineInterpolated($"Validating definition [blue]{settings.DefinitionName}[/] located in [blue]{settings.DefinitionFolder}[/]");

        if (string.IsNullOrEmpty(settings?.DefinitionName) || string.IsNullOrEmpty(settings?.DefinitionFolder))
        {
            _console.MarkupLine("[red]Definition Name or Folder is missing.[/]");
            return 1;
        }

        var filename = Path.Combine(settings.DefinitionFolder, Targets.Default, settings.DefinitionName + ".dll");
        if (!System.IO.File.Exists(filename))
        {
            _console.MarkupLine($"Definition {filename} does not exist.");
            return 1;
        }

        var item = _definitionLoader.Load(filename);
        if (item?.Instance() is null)
        {
            _console.MarkupLine($"Definition {filename} was unable to be loaded");
            return 1;
        }

        try
        {
            _console.WriteLine("Getting instance...");
            var instance = item.Instance();
            _console.WriteLine("Reading and creating module");
            instance.Create();
            _console.MarkupLine("Validation Result: [Green]Success[/]");

            return 0;
        }
        catch (Exception ex)
        {
            _console.MarkupLine($"ServiceData was unable to be created: {ex.Message}");
            return 1;
        }
    }
}