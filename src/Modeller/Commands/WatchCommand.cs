using System.Runtime.CompilerServices;

using Modeller.NET.Tool.Core;
using Modeller.NET.Tool.Generators;
using Modeller.Parsers.Models;

namespace Modeller.NET.Tool.Commands;

// ReSharper disable once ClassNeverInstantiated.Global
internal class WatchCommand(IAnsiConsole console, FileSystemMonitor monitor)
    : AsyncCommand<WatchSettings>
{
    private CancellationTokenSource? _cts;
    private DirectoryInfo _definitionFolder = null!;
    private LeesBucket _changes = null!;
    
    public override async Task<int> ExecuteAsync(CommandContext context, WatchSettings settings)
    {
        if (!Directory.Exists(settings.DefinitionFolder))
        {
            console.MarkupLine($"[bold red]Definition folder '{settings.DefinitionFolder}' does not exist.[/]");
            return 1;
        }
        console.MarkupLine("[bold yellow]Starting Modeller watcher...[/]");

        _definitionFolder = new(settings.DefinitionFolder);
        if (_cts is not null) await _cts.CancelAsync();
        _cts = new();

        var directoryEnumerator = new DirectoryEnumerator();
        var builders = await IterateDirectory(directoryEnumerator, _cts.Token).ToListAsync();
        _changes = new(builders, EnterpriseUpdated);
        _changes.Start();

        var monitorTask = monitor.MonitorAsync(_changes, console, settings.DefinitionFolder, _cts.Token);
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
    
    private async IAsyncEnumerable<Builder> IterateDirectory(DirectoryEnumerator
        directoryEnumerator, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var file in directoryEnumerator.EnumerateFilesAsync(_definitionFolder.FullName, cancellationToken))
        {
            if (cancellationToken.IsCancellationRequested) break;

            console.MarkupLine($"[bold green]Processing file:[/] [blue]{file}[/]");

            var o = await FileProcessor.ProcessFile(file);
            if (o is not null) yield return o;
        }
    }
    
    private void EnterpriseUpdated(Enterprise enterprise)
    {
        foreach (var e in enterprise.Entities)
        {
            var g = new EntityGenerator(enterprise, e);
            AnsiConsole.WriteLine(g.Generate());
        }
    }
}

internal static class FileProcessor
{
    internal static async Task<Builder?> ProcessFile(string file)
    {
        var builder = Path.GetExtension(file) switch
        {
            ".project" => EntityParser.ParseProject,
            ".entity" => EntityParser.ParseEntity,
            ".key" => EntityParser.ParseEntityKey,
            ".domain" => EntityParser.ParseDomain,
            ".enum" => EntityParser.ParseEnum,
            ".flags" => EntityParser.ParseFlag,

            ".service" => EntityParser.ParseService,
            ".endpoint" => EntityParser.ParseEndpoint,
            ".type" => EntityParser.ParseRpcType,
            ".rpc" => EntityParser.ParseRpc,
            _ => null
        };
        if (builder is null) return null;

        var content = await File.ReadAllTextAsync(file);
        return builder(content);
    }
}