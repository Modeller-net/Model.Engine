namespace Modeller;

public abstract partial class Parser<TToken, T>
{
    /// <summary>
    /// Returns a parser which runs the current parser and applies a selector function.
    /// The selector function receives a <see cref="ReadOnlySpan{TToken}"/> as its first argument, and the result of the current parser as its second argument.
    /// The <see cref="ReadOnlySpan{TToken}"/> represents the sequence of input tokens which were consumed by the parser.
    ///
    /// This allows you to write "pattern"-style parsers which match a sequence of tokens and return a view of the part of the input stream which they matched.
    ///
    /// This function is an alternative name for <see cref="Slice"/>.
    /// </summary>
    /// <param name="selector">
    /// A selector function which computes a result of type <typeparamref name="Tu"/>.
    /// The arguments of the selector function are a <see cref="ReadOnlySpan{TToken}"/> containing the sequence of input tokens which were consumed by this parser,
    /// and the result of this parser.
    /// </param>
    /// <typeparam name="Tu">The result type.</typeparam>
    /// <returns>A parser which runs the current parser and applies a selector function.</returns>
    public Parser<TToken, Tu> MapWithInput<Tu>(ReadOnlySpanFunc<TToken, T, Tu> selector)
    {
		ArgumentNullException.ThrowIfNull(selector);

		return new MapWithInputParser<TToken, T, Tu>(this, selector);
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:FileMayOnlyContainASingleType",
    Justification = "This class belongs next to the accompanying API method"
)]
internal class MapWithInputParser<TToken, T, Tu>(Parser<TToken, T> parser, ReadOnlySpanFunc<TToken, T, Tu> selector)
    : Parser<TToken, Tu>
{
    public override sealed bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, [MaybeNullWhen(false)] out Tu result)
    {
        var start = state.Location;

        var bookmark = state.Bookmark();  // don't discard input buffer

        if (!parser.TryParse(ref state, ref expected, out var result1))
        {
            state.DiscardBookmark(bookmark);
            result = default;
            return false;
        }

        var delta = state.Location - start;
        result = selector(state.LookBehind(delta), result1);

        state.DiscardBookmark(bookmark);

        return true;
    }
}
