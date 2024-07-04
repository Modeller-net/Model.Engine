using Modeller.NET.Tool.Core;

namespace Modeller.NET.Tool.Commands;

internal class WatchCommand(IAnsiConsole console, ILoader<IDefinitionItem> definitionLoader, ILogger<WatchCommand> logger)
	 : Command<WatchSettings>
{
	public override int Execute(CommandContext context, WatchSettings settings)
	{
		try
		{
			//return definitionLoader.Watch(settings) ? 0 : 1;
			return 0;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Watch Command - OnExecute");
			console.MarkupLine($"[red]{ex.Message}[/]");
			return 1;
		}
		finally
		{
			logger.LogDebug("Watch Command - complete");
			console.WriteLine("Watch Output Complete");
		}
	}
}