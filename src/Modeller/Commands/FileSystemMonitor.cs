using System.Reactive;
using System.Reactive.Linq;

namespace Modeller.NET.Tool.Commands;

internal sealed class FileSystemMonitor(ILogger<FileSystemMonitor> logger)
{
    private readonly FileProcessor _fileProcessor = new();

    public async Task MonitorAsync(LeesBucket changes, IAnsiConsole console, string directoryPath, CancellationToken 
            cancellationToken)
    {
        if (!Directory.Exists(directoryPath))
        {
            logger.LogError("Definition folder '{DirectoryPath}' does not exist", directoryPath);
            return;
        }

        var fileSystemWatcher = InitializeFileSystemWatcher(new DirectoryInfo(directoryPath));
        using var subscription = CreateObservables(fileSystemWatcher)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .Subscribe(OnNext);

        fileSystemWatcher.EnableRaisingEvents = true;

        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            // Handle the cancellation
        }
        finally
        {
            fileSystemWatcher.EnableRaisingEvents = false;
            console.MarkupLine("[bold yellow]Stopped Modeller watcher[/]");
            logger.LogInformation("Stopped Modeller watcher");
        }

        return;

        async void OnNext(EventPattern<FileSystemEventArgs> evt)
        {
            try
            {
                var e = evt.EventArgs;
                console.MarkupLine($"[green]{e.ChangeType}[/] [yellow]{e.FullPath}[/]");

                var b = await _fileProcessor.ProcessFile(e.FullPath);
                if(b is not null)
                    changes.Add(b);

                logger.LogInformation("{ChangeType} {FullPath}", e.ChangeType, e.FullPath);
            }
            catch (Exception ex)
            {
                console.MarkupLine($"[red]{ex.Message}[/]");
                logger.LogError(ex, "An error occurred while processing file system events");
            }
        }
    }

    private static FileSystemWatcher InitializeFileSystemWatcher(DirectoryInfo rootFolder) =>
        new(rootFolder.FullName)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
        };

    private static IObservable<EventPattern<FileSystemEventArgs>> CreateObservables(FileSystemWatcher watcher)
    {
        var created = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
            handler => watcher.Created += handler,
            handler => watcher.Created -= handler);

        var changed = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
            handler => watcher.Changed += handler,
            handler => watcher.Changed -= handler);

        var deleted = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
            handler => watcher.Deleted += handler,
            handler => watcher.Deleted -= handler);

        return Observable.Merge(created, changed, deleted);
    }
}