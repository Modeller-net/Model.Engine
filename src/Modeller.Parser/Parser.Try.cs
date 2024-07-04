﻿namespace Modeller;

public static partial class Parser
{
	/// <summary>
	/// Creates a parser which applies <paramref name="parser"/> and backtracks upon failure.
	/// </summary>
	/// <typeparam name="TToken">The type of tokens in the parser's input stream.</typeparam>
	/// <typeparam name="T">The return type of the parser.</typeparam>
	/// <param name="parser">The parser.</param>
	/// <returns>A parser which applies <paramref name="parser"/> and backtracks upon failure.</returns>
	public static Parser<TToken, T> Try<TToken, T>(Parser<TToken, T> parser) => new TryParser<TToken, T>(parser);
}

internal sealed class TryParser<TToken, T>(Parser<TToken, T> parser) : Parser<TToken, T>
{
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, [MaybeNullWhen(false)] out T result)
	{
		// start buffering the input
		var bookmark = state.Bookmark();
		if (!parser.TryParse(ref state, ref expected, out result))
		{
			// return to the start of the buffer and discard the bookmark
			state.Rewind(bookmark);
			return false;
		}

		// discard the buffer
		state.DiscardBookmark(bookmark);
		return true;
	}
}
