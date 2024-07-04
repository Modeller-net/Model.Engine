namespace Domain;

/// <summary>
/// Defines a writer that can take a instance item and persist it.
/// </summary>
public interface IWriter
{
    /// <summary>
    /// Writes the specified instance to a file.
    /// </summary>
    /// <param name="name">The name of the instance (i.e. the path and name to the file system instance</param>
    /// <param name="instance">The instance to be written</param>
    void Write<T>(string name, T instance);
}