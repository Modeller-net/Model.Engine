namespace Model.Domain;

public static class SettingsExtensions
{
    /// <summary>
    /// Gets the default file information based on the persistence type.
    /// </summary>
    public static FileInfo GetDefaultFileInfo(this Settings settings, IFilePersistenceType<Settings> type, string? nameOverride = null) =>
        new(Path.Combine(settings.OutputFolder, $"{nameOverride ?? Settings.DefaultFilename}{type.ToFileExtension()}"));

    public static string SerialisedContent(this Settings settings, IFilePersistenceType<Settings> type) =>
        type.Serialize(settings);

    // /// <summary>
    // /// Updates the runtime state within the SettingsContext.
    // /// </summary>
    // public static SettingsContext WithUpdatedState(this SettingsContext context, SettingsState state) =>
    //     context with { State = state };

    /// <summary>
    /// Saves the Settings to a file using the specified FilePersistenceType.
    /// </summary>
    public static SettingResponse Save(this Settings settings, IFilePersistenceType<Settings> type, string? filenameOverride = null)
    {
        var filePath = Path.Combine(settings.OutputFolder, $"{filenameOverride ?? Settings.DefaultFilename}{type.ToFileExtension()}");
        var info = new FileInfo(filePath);

        try
        {
            var serializedContent = type.Serialize(settings);

            // Ensure the output directory exists
            Directory.CreateDirectory(settings.OutputFolder);

            File.WriteAllText(filePath, serializedContent);
            return new SettingResponse(settings, info, true, "Saved successfully");
        }
        catch (Exception ex)
        {
            return new SettingResponse(settings, info, false, ex.Message);
        }

    }

    /// <summary>
    /// Loads the Settings from a file using the specified FilePersistenceType.
    /// If the file does not exist, default Settings are returned.
    /// </summary>
    public static SettingResponse Load(this Settings settings, IFilePersistenceType<Settings> type, string? filenameOverride = null)
    {
        var filePath = Path.Combine(settings.OutputFolder, $"{filenameOverride ?? Settings.DefaultFilename}{type.ToFileExtension()}");
        var info = new FileInfo(filePath);

        if (!File.Exists(filePath))
        {
            return new SettingResponse(Settings.Default, info, true, $"Created default settings as file didn't exist '{filePath}'");
        }

        try
        {
            var serializedContent = File.ReadAllText(filePath);
            return new SettingResponse(type.Deserialize(serializedContent), info, true, "Settings loaded successfully");
        }
        catch (Exception ex)
        {
            return new SettingResponse(Settings.Default, info, false, $"Failed to load settings '{filePath}': {ex.Message}");
        }
    }
}