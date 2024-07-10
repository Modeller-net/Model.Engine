using System.Runtime.CompilerServices;
using Modeller.Parsers.Models;

namespace Modeller.NET.Tool.Commands;

public class DirectoryEnumerator
{
    public async IAsyncEnumerable<string> EnumerateFilesAsync(string directory,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(directory))
            throw new ArgumentException("Directory path cannot be null or whitespace.", nameof(directory));

        if (!Directory.Exists(directory))
            throw new DirectoryNotFoundException($"Directory not found: {directory}");

        await foreach (var file in EnumerateFilesRecursiveAsync(directory, cancellationToken))
        {
            yield return file;
        }
    }

    private async IAsyncEnumerable<string> EnumerateFilesRecursiveAsync(string directory,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var file in Directory.EnumerateFiles(directory))
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            yield return file;
            await Task.Yield(); // Yield control to allow for cancellation
        }

        foreach (var subDirectory in Directory.EnumerateDirectories(directory))
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            await foreach (var file in EnumerateFilesRecursiveAsync(subDirectory, cancellationToken))
            {
                yield return file;
            }
        }
    }
}

public static class BuilderExtensions
{
    public static async Task ProcessFileAsync(this Builder builder, string filePath)
    {
        var content = await File.ReadAllTextAsync(filePath);
        
    }
}