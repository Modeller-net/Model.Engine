namespace Domain;

/// <summary>
/// The settings service
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Validate the filepath
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    System.IO.FileInfo ValidateFilepath(string? filepath);

    /// <summary>
    /// Update the configuration and return the settings
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <returns></returns>
    (Settings setting, bool loaded) UpdateAndReturn(System.IO.FileInfo fileInfo);

    /// <summary>
    /// Save the settings
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <param name="settings"></param>
    void SaveSettings(System.IO.FileInfo fileInfo, Settings settings);
}