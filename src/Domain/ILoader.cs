namespace Domain;

/// <summary>
/// Represents an item that can be loaded from a file.
/// </summary>
/// <typeparam name="T">The type that will be loaded</typeparam>
public interface ILoader<T>
{
    /// <summary>
    /// Loads the specified file path.
    /// </summary>
    /// <param name="filePath">The path to the file</param>
    /// <returns>An instance of T</returns>
    T? Load(string filePath);

    /// <summary>
    /// Loads the specified file path. This method will return false if the file does not exist.
    /// </summary>
    /// <param name="filePath">The path to the file</param>
    /// <param name="instances">The instance of T if found</param>
    /// <returns>True if loaded, otherwise false</returns>
    
    bool TryLoad(string? filePath, [MaybeNullWhen(false)] out T? instances);
}