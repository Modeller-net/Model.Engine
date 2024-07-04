using Spectre.Console.Json;

namespace Modeller.NET.Tool.Commands;

internal sealed class SettingsCommand(IAnsiConsole console, ISettingsService settingsService, ILogger<SettingsCommand> logger) : Command<SettingsCommand.Settings>
{
    private readonly IAnsiConsole _console = console ?? throw new ArgumentNullException(nameof(console));
    private readonly ILogger<SettingsCommand> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ISettingsService _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));

    public class Settings : LogCommandSettings
    {
        [CommandOption("--filepath")]
        [Description("Folder where Settings.json is located.")]
        public string? FilePath { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        _logger.LogTrace("Generator Settings Command - OnExecute");

        var validFileInfo = _settingsService.ValidateFilepath(settings.FilePath);
        var found = validFileInfo.Exists ? "found" : "was not found";
        _console.MarkupInterpolated($"[blue]{validFileInfo.FullName}[/] {found}");
        _logger.LogInformation("Filepath: {Filepath} - {Found}", validFileInfo.FullName, found);

        var (setting, loaded) = _settingsService.UpdateAndReturn(validFileInfo);

        var json = JsonSerializer.Serialize(setting);
        _logger.LogInformation("{Message}", json);

        _console.MarkupLine(loaded
            ? " and [green]loaded[/]"
            : " but [green]created[/] with defaults");
        _console.WriteLine();
        _console.Write(
            new Panel(new JsonText(json))
                .Header("contents")
                .Collapse()
                .RoundedBorder()
                .BorderColor(Color.Yellow)
        );

        return 0;
    }
}