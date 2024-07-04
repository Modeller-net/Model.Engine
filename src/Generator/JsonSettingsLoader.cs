using System.Text.Json;

namespace Generator;

public sealed class JsonSettingsLoader(ILogger<JsonSettingsLoader> logger) : ILoader<Settings>
{
    private readonly ILogger<JsonSettingsLoader> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private static Settings Load(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
        {
            throw new System.IO.FileNotFoundException($"Settings file '{filePath}' does not exist.");
        }

        using var reader = System.IO.File.OpenText(filePath);
        var setting = JsonSerializer.Deserialize<Settings>(reader.ReadToEnd());
        return setting ?? throw new System.IO.FileNotFoundException($"Settings file '{filePath}' corrupt or in the incorrect format.");
    }

    Settings ILoader<Settings>.Load(string filePath)
    {
        return Load(filePath);
    }

    bool ILoader<Settings>.TryLoad(string? filePath, out Settings instances)
    {
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            try
            {
                instances = Load(filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load settings");
            }
        }
        else
        {
            _logger.LogInformation("Skipped settings load because no path/file was found.  Using defaults instead.");
        }
        instances = null!;
        return false;
    }
}