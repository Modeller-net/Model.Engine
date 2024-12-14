using Model.Domain;

namespace Model.Commands;

public sealed class ListDefinitionsCommand(Settings settings, IAnsiConsole console, ILoaderAsync<IDefinitionMetaData> definitionLoader, ILogger<ListDefinitionsCommand> logger)
    : Command<ListDefinitionsCommand.ListSettings>
{
    private Settings _settings = settings;

    public sealed class ListSettings : LogCommandSettings
    {
        [CommandOption("--folder")]
        [Description("Folder where Definitions are located.")]
        public string? DefinitionFolder{ get; init; }

        [CommandOption("--target")]
        [Description("The target framework to use (default: net9.0).")]
        [DefaultValue("net9.0")]
        public string? Target { get; init; }
    }

    public override int Execute(CommandContext context, ListSettings settings)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(settings.DefinitionFolder))
                _settings = _settings with { DefinitionFolder = settings.DefinitionFolder };
            if (!string.IsNullOrWhiteSpace(settings.Target))
                _settings = _settings with { Target = settings.Target };

            logger.LogDebug("ListDefinitionsCommand - start");

            console.MarkupLine($"[bold yellow]List[/] definitions in [blue]{_settings.TemplateFolder}[/] that target [blue]{_settings.Target}[/]");

            var table = new Table().Expand().BorderColor(Color.Grey);
            table.AddColumn("[yellow]Usage[/]");
            table.AddColumn("[yellow]Name[/]");
            table.AddColumn("[yellow]Description[/]");
            table.AddColumn("[yellow]Version[/]");

            console.Live(table)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .Cropping(VerticalOverflowCropping.Bottom)
                .StartAsync(async (ctx) =>
                {
                    var list = await definitionLoader.LoadAsync(_settings.DefinitionFolder);
                    foreach (var i in list)
                    {
                        table.AddRow(i.AbbreviatedFileName, i.Name, i.Description, i.Version);
                    }
                });

            return 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ListDefinitionsCommand - failed");
            return 1;
        }
        finally
        {
            logger.LogDebug("ListDefinitionsCommand - complete");
        }
    }
}