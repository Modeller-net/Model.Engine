namespace Modeller;

public static partial class Parser
{
	/// <summary>
	/// If <paramref name="parser"/> succeeds, <c>Lookahead(parser)</c> backtracks,
	/// behaving as if <paramref name="parser"/> had not consumed any input.
	/// No backtracking is performed upon failure.
	/// </summary>
	/// <param name="parser">The parser to look ahead with.</param>
	/// <typeparam name="TToken">The type of the tokens in the parser's input stream.</typeparam>
	/// <typeparam name="T">The type of the value returned by the parser.</typeparam>
	/// <returns>A parser which rewinds the input stream if <paramref name="parser"/> succeeds.</returns>
	public static Parser<TToken, T> Lookahead<TToken, T>(Parser<TToken, T> parser)
	{
        ArgumentNullException.ThrowIfNull(parser);

        return new LookaheadParser<TToken, T>(parser);
	}
}

[SuppressMessage(
	"StyleCop.CSharp.MaintainabilityRules",
	"SA1402:FileMayOnlyContainASingleType",
	Justification = "This class belongs next to the accompanying API method"
)]
internal sealed class LookaheadParser<TToken, T>(Parser<TToken, T> parser) : Parser<TToken, T>
{
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, [MaybeNullWhen(false)] out T result)
	{
		var bookmark = state.Bookmark();

		if (parser.TryParse(ref state, ref expected, out result))
		{
			state.Rewind(bookmark);
			return true;
		}

		state.DiscardBookmark(bookmark);
		return false;
	}
}
