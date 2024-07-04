namespace Domain;

/// <summary>
/// Represents a definition writer.
/// </summary>
public interface IDefinitionWriter
{
    /// <summary>
    ///  Writes the specified instance to a file.
    /// </summary>
    /// <param name="rootFolder">The root folder</param>
    /// <param name="instance">The instance to be written</param>
    void Write(string rootFolder, Enterprise instance);
}