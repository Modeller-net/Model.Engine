using System.IO.Abstractions;

namespace Generator;

public sealed class GenericWriter(IFileSystem fileSystem, ILogger<GenericWriter> logger) : IWriter
{
    private ILogger<GenericWriter> _logger = logger;

    public void Write<T>(string filePath, T instance)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        if (instance is null)
        {
            _logger.LogWarning("Generic Writer: Skipping instance {Type} as it is null. Filepath: {File}", typeof(T).Name, filePath);
            return;
        }

        _logger.LogDebug("Generic Writer: Writing {Type} to '{File}'", typeof(T).Name, filePath);
        fileSystem.File.WriteAllText(filePath, instance.ToYaml(false));
    }
}
