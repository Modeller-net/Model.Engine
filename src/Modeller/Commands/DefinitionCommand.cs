using Modeller.NET.Tool.Core;

namespace Modeller.NET.Tool.Commands;

public sealed class DefinitionCommand(IAnsiConsole console, IDefinitions definitions, ILogger<DefinitionCommand> logger)
    : Command<DefinitionSettings>
{
    public override int Execute(CommandContext context, DefinitionSettings settings)
    {
        try
        {
            return definitions.Create(settings) ? 0 : 1;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Definition Command - OnExecute");
            console.MarkupLine($"[red]{ex.Message}[/]");
            return 1;
        }
        finally
        {
            logger.LogDebug("Definition Command - complete");
            console.WriteLine("Definition Output Complete");
        }
    }
}