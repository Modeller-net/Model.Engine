using System.Runtime.CompilerServices;

namespace Modeller.NET.Tool.Commands;

public class DirectoryEnumerator
{
    public IAsyncEnumerable<string> EnumerateFilesAsync(string directory, CancellationToken cancellationToken)
    {
        return EnumerateFilesRecursiveAsync(directory, cancellationToken);
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
