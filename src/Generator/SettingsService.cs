using System.Text;
using System.Text.Json;

namespace Generator;

/// <inheritdoc />
/// <summary>
/// Initializes a new instance of the <see cref="SettingsService"/> class.
/// </summary>
/// <param name="logger"></param>
/// <param name="settingsLoader"></param>
/// <exception cref="ArgumentNullException"></exception>
public class SettingsService(ILogger<SettingsService> logger, ILoader<Settings> settingsLoader) : ISettingsService
{
    public const string DefaultFilename = "BuildSettings.json";

    private readonly ILogger<SettingsService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ILoader<Settings> _settingsLoader = settingsLoader ?? throw new ArgumentNullException(nameof(settingsLoader));

    /// <inheritdoc />
    public FileInfo ValidateFilepath(string? filepath)
    {
        var file = string.IsNullOrWhiteSpace(filepath) ? Path.Combine(Settings.DefaultOutputFolder.FullName, DefaultFilename) : filepath;

        // validate the file and directory
        var directory = Path.GetDirectoryName(Path.GetFullPath(file)) ?? Directory.GetCurrentDirectory();
        var filename = Path.GetFileName(file);

        // finally combine the directory and filename
        return new(Path.Combine(directory, filename));
    }

    /// <inheritdoc />
    public (Settings setting, bool loaded) UpdateAndReturn(FileInfo fileInfo)
    {
        if (_settingsLoader.TryLoad(fileInfo.FullName, out var instance))
        {
            return (instance!, true);
        }

        instance = Settings.Default;
        SaveSettings(fileInfo, instance);
        return (instance, false);
    }

    /// <inheritdoc />
    public void SaveSettings(FileInfo fileInfo, Settings settings)
    {
        try
        {
            File.WriteAllText(fileInfo.FullName, JsonSerializer.Serialize(settings), Encoding.UTF8);
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);

            Console.Error.WriteLine(ex.Message);
        }
    }
}