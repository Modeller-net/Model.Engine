namespace Modeller;

public static partial class Parser<TToken>
{
	/// <summary>
	/// A parser which returns the number of input tokens which have been consumed.
	/// </summary>
	public static Parser<TToken, int> CurrentOffset { get; }
		= new CurrentOffsetParser<TToken>();
}

[SuppressMessage(
	"StyleCop.CSharp.MaintainabilityRules",
	"SA1402:FileMayOnlyContainASingleType",
	Justification = "This class belongs next to the accompanying API method"
)]
internal sealed class CurrentOffsetParser<TToken> : Parser<TToken, int>
{
	public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, out int result)
	{
		result = state.Location;
		return true;
	}
}
