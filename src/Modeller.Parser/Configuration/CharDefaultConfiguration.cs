namespace Modeller.Configuration;

/// <summary>
/// A default configuration for textual input.
/// </summary>
public class CharDefaultConfiguration : DefaultConfiguration<char>
{
	/// <summary>
	/// The shared global instance of <see cref="CharDefaultConfiguration"/>.
	/// </summary>
	public static new IConfiguration<char> Instance { get; } = new CharDefaultConfiguration();

	/// <summary>
	/// Handles newlines and tab stops.
	/// </summary>
	public override Func<char, SourcePosDelta> SourcePosCalculator { get; }
		= token => token switch
        {
            '\n' => SourcePosDelta.NewLine,
            '\t' => new(0, 4),
            _ => SourcePosDelta.OneCol
        };
}