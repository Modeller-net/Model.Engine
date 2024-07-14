using Modeller.NET.Tool.Core;
using Modeller.Parsers.Models;

namespace Modeller.NET.Tool.Commands;

internal class WatchCommand(IAnsiConsole console, FileSystemMonitor monitor)
    : AsyncCommand<WatchSettings>
{
    private CancellationTokenSource? _cts;

    public override async Task<int> ExecuteAsync(CommandContext context, WatchSettings settings)
    {
        if (!Directory.Exists(settings.DefinitionFolder))
        {
            console.MarkupLine($"[bold red]Definition folder '{settings.DefinitionFolder}' does not exist.[/]");
            return 1;
        }

        console.MarkupLine("[bold yellow]Starting Modeller watcher...[/]");

        if(_cts is not null) await _cts.CancelAsync();
        _cts = new CancellationTokenSource();

        var directoryEnumerator = new DirectoryEnumerator();
        await IterateDirectory(settings, directoryEnumerator);

        var monitorTask = monitor.MonitorAsync(console, settings.DefinitionFolder, _cts.Token);
        console.MarkupLine("[bold green]Press [red]Ctrl+C[/] to exit.[/]");
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            _cts?.Cancel();
        };

        try
        {
            monitorTask.Wait();
        }
        catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is TaskCanceledException)) { }

        console.MarkupLine("[bold yellow]Watcher Command completed.[/]");
        return 0;
    }
    async private Task IterateDirectory(WatchSettings settings, DirectoryEnumerator directoryEnumerator)
    {
        try
        {
            await foreach (var file in directoryEnumerator.EnumerateFilesAsync(settings.DefinitionFolder, _cts.Token))
            {
                if (_cts.Token.IsCancellationRequested)
                    break;

                try
                {
                    console.MarkupLine($"[bold green]Processing file:[/] [blue]{file}[/]");
                    var builder = Path.GetExtension(file) switch
                    {
                        ".entity" => EntityParser.ParseEntity,
                        ".domain" => EntityParser.ParseDomain,
                        ".enum" => EntityParser.ParseEnum,
                        ".flags" => EntityParser.ParseFlag,
                        ".service" => EntityParser.ParseService,
                        ".endpoint" => EntityParser.ParseEndpoint,
                        ".key" => EntityParser.ParseEntityKey,
                        ".type" => EntityParser.ParseRpcType,
                        ".rpc" => EntityParser.ParseRpc,
                        _ => null
                    };
                    if (builder is not null)
                    {
                        var content = await File.ReadAllTextAsync(file);
                        var x = builder(content);
                    }
                    else
                    {
                        console.MarkupLine($"[red]Unsupported file type: {Path.GetExtension(file)}[/]");
                    }
                }
                catch (Exception ex)
                {
                    console.MarkupLine($"[red]{ex.Message}[/]");
                }
            }
        }
        catch (OperationCanceledException)
        {
            console.WriteLine("Operation was cancelled");
        }
    }
}