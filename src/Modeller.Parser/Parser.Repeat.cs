namespace Modeller;

public static partial class Parser
{
    /// <summary>
    /// Creates a parser which applies <paramref name="parser"/> <paramref name="count"/> times,
    /// packing the resulting <c>char</c>s into a <c>string</c>.
    ///
    /// <para>
    /// Equivalent to <c>parser.Repeat(count).Select(string.Concat)</c>.
    /// </para>
    /// </summary>
    /// <typeparam name="TToken">The type of tokens in the parser's input stream.</typeparam>
    /// <param name="parser">The parser.</param>
    /// <param name="count">The number of times to apply the parser.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> was less than 0.</exception>
    /// <returns>
    /// A parser which applies <paramref name="parser"/> <paramref name="count"/> times,
    /// packing the resulting <c>char</c>s into a <c>string</c>.
    /// </returns>
    public static Parser<TToken, string> RepeatString<TToken>(this Parser<TToken, char> parser, int count)
    {
		ArgumentNullException.ThrowIfNull(parser);

		if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative");
        }

        return new RepeatStringParser<TToken>(parser, count);
    }
}

public abstract partial class Parser<TToken, T>
{
    /// <summary>
    /// Creates a parser which applies the current parser <paramref name="count"/> times.
    /// </summary>
    /// <param name="count">The number of times to apply the current parser.</param>
    /// <exception cref="InvalidOperationException"><paramref name="count"/> is less than 0.</exception>
    /// <returns>A parser which applies the current parser <paramref name="count"/> times.</returns>
    public Parser<TToken, IEnumerable<T>> Repeat(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative");
        }

        return Parser<TToken>.Sequence(Enumerable.Repeat(this, count));
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:FileMayOnlyContainASingleType",
    Justification = "This class belongs next to the accompanying API method"
)]
internal sealed class RepeatStringParser<TToken>(Parser<TToken, char> parser, int count) : Parser<TToken, string>
{
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, [MaybeNullWhen(false)] out string result)
    {
        var builder = new InplaceStringBuilder(count);

        for (var i = 0; i < count; i++)
        {
            var success = parser.TryParse(ref state, ref expected, out var result1);

            if (!success)
            {
                result = null;
                return false;
            }

            builder.Append(result1);
        }

        result = builder.ToString();
        return true;
    }
}
