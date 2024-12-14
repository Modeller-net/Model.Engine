using Model.Domain;

namespace Model.Commands;

public sealed class SettingsCommand(IAnsiConsole console, IEnumerable<IFilePersistenceType<Settings>> persistenceTypes, ILogger<SettingsCommand> logger)
    : Command<ModelSettingsCommandSettings>
{
    public override int Execute(CommandContext context, ModelSettingsCommandSettings commandSettings)
    {
        logger.LogTrace("Generator Settings Command - OnExecute");

        var newSettings = Settings.Default;
        var persistenceType = persistenceTypes.FirstOrDefault(x => x.Value.Equals(commandSettings.Type, StringComparison.OrdinalIgnoreCase));
        if(persistenceType is null)
        {
            console.MarkupLine($"[red]Persistence type '{commandSettings.Type}' not found[/]");
            return 1;
        }

        var currentFile = newSettings.GetDefaultFileInfo(persistenceType, commandSettings.Name);
        var found = currentFile.Exists ? "found" : "was not found";
        console.MarkupInterpolated($"[blue]{currentFile.FullName}[/] {found}");
        logger.LogInformation("Filepath: {Filepath} - {Found}", currentFile.FullName, found);

        if (currentFile.Exists)
        {
            var filename = Path.GetFileNameWithoutExtension(currentFile.Name);
            var response = newSettings.Load(persistenceType, filename);
            if(response.Success)
            {
                newSettings = response.Instance;
            }
            else
            {
                console.MarkupLine($"[red]Failed to load settings: - {response.Message}[/]");
                return 1;
            }
        }

        var result = newSettings.Save(persistenceType);
        var content = newSettings.SerialisedContent(persistenceType);

        logger.LogInformation("{Message}", content);

        console.MarkupLine($" and [green]{result.Message}[/]");
        console.WriteLine();
        console.Write(
            new Panel(new Text(content))
                .Header("contents")
                .Collapse()
                .RoundedBorder()
                .BorderColor(Color.Yellow)
        );

        return 0;
    }
}