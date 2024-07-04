using System.IO.Abstractions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Modeller.NET.Tool.Core;

internal sealed class Definitions(
    IAnsiConsole console,
    ILoader<IDefinitionItem> definitionLoader,
    IDefinitionWriter writer,
    IFileSystem fileSystem,
    ILogger logger
) : IDefinitions
{
    bool IDefinitions.Create(DefinitionSettings settings)
    {
        logger.LogDebug("Definition Command - OnExecute");

        if (string.IsNullOrEmpty(settings.DefinitionName) || string.IsNullOrEmpty(settings.DefinitionFolder))
        {
            logger.LogWarning("Definition Command - Definition Name or Folder is missing");
            console.MarkupLine("  [red]Definition Name or Folder is missing.[/]");
            {
                return false;
            }
        }

        var filename = Path.Combine(settings.DefinitionFolder ?? "", settings.Target, settings.DefinitionName + ".dll");
        if (!fileSystem.File.Exists(filename))
        {
            logger.LogWarning("Definition Command - Definition {Filename} does not exist", filename);
            console.MarkupLineInterpolated($"  [red]Definition {filename} does not exist.[/]");
            {
                return false;
            }
        }

        logger.LogInformation("Definition: {Definition}", $"{settings.DefinitionFolder}/{settings.Target}/{settings.DefinitionName}.dll");
        logger.LogInformation("Output: {OutputFolder}", $"{settings.OutputFolder}");

        console.WriteLine("Definiton Extract Started");
        console.MarkupLineInterpolated($"  Definition: [blue]{settings.DefinitionFolder}/{settings.Target}/{settings.DefinitionName}.dll[/]");
        console.MarkupLineInterpolated($"  Output: [blue]{settings.OutputFolder}[/]");

        // create a module with a complex data entity
        var definitionItem = definitionLoader.Load(filename);
        var instance = definitionItem?.Instance();
        if (instance is null)
        {
            console.MarkupLineInterpolated($"  [red]Definition {filename} was unable to be loaded[/]");
            {
                return false;
            }
        }

        var module = instance.Create();

        // write to disk
        var rootFolder = Path.Combine(settings.OutputFolder, module.Project);

        writer.Write(rootFolder, module);

        console.MarkupLine($"Definition successful - output at [blue]{rootFolder}[/]");
        return true;
    }
}