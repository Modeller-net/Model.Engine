using Model.Domain;

namespace Model.Commands;

public sealed class ListTemplatesCommand(
    Settings settings,
    IAnsiConsole console,
    ILoaderAsync<IMetadata> generatorLoader,
    ILogger<ListTemplatesCommand> logger)
    : Command<ListTemplatesCommand.ListSettings>
{
    private Settings _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    private readonly IAnsiConsole _console = console ?? throw new ArgumentNullException(nameof(console));
    private readonly ILoaderAsync<IMetadata> _generatorLoader = generatorLoader ?? throw new ArgumentNullException(nameof(generatorLoader));
    private readonly ILogger<ListTemplatesCommand> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public sealed class ListSettings : LogCommandSettings
    {
        [CommandOption("--folder" )]
        [Description("Folder where Templates are located.")]
        public string? TemplateFolder { get; init; }

        [CommandOption("--target")]
        [Description("The target framework to use (default: net9.0).")]
        [DefaultValue("net9.0")]
        public string? Target { get; init; }
    }

    public override int Execute(CommandContext context, ListSettings settings)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(settings.TemplateFolder))
                _settings = _settings with { TemplateFolder = settings.TemplateFolder };
            if (!string.IsNullOrWhiteSpace(settings.Target))
                _settings = _settings with { Target = settings.Target };

            _logger.LogDebug("ListTemplatesCommand - start");

            _console.MarkupLine($"[bold yellow]List[/] templates in [blue]{_settings.TemplateFolder}[/] that target [blue]{_settings.Target}[/]");

            var table = new Table().Expand().BorderColor(Color.Grey);
            table.AddColumn("[yellow]Usage[/]");
            table.AddColumn("[yellow]Name[/]");
            table.AddColumn("[yellow]Description[/]");
            table.AddColumn("[yellow]Version[/]");

            _console.Live(table)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .Cropping(VerticalOverflowCropping.Bottom)
                .StartAsync(async (ctx) =>
                {
                    var list = await _generatorLoader.LoadAsync(_settings.TemplateFolder);
                    foreach (var i in list)
                    {
                        table.AddRow(i.AbbreviatedFileName, i.Name, i.Description, i.Version);
                    }
                });

            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ListTemplatesCommand - failed");
            return 1;
        }
        finally
        {
            _logger.LogDebug("ListTemplatesCommand - complete");
        }
    }
}