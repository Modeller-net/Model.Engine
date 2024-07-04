namespace Modeller;

public abstract partial class Parser<TToken, T>
{
    /// <summary>
    /// Creates a parser which applies the current parser repeatedly, interleaved with a specified parser.
    /// The resulting parser ignores the return value of the separator parser.
    /// </summary>
    /// <typeparam name="Tu">The return type of the separator parser.</typeparam>
    /// <param name="separator">A parser which parses a separator to be interleaved with the current parser.</param>
    /// <returns>A parser which applies the current parser repeatedly, interleaved by <paramref name="separator"/>.</returns>
    public Parser<TToken, IEnumerable<T>> Separated<Tu>(Parser<TToken, Tu> separator)
    {
		ArgumentNullException.ThrowIfNull(separator);

		return SeparatedAtLeastOnce(separator)
            .Or(ReturnEmptyEnumerable);
    }

    /// <summary>
    /// Creates a parser which applies the current parser at least once, interleaved with a specified parser.
    /// The resulting parser ignores the return value of the separator parser.
    /// </summary>
    /// <typeparam name="Tu">The return type of the separator parser.</typeparam>
    /// <param name="separator">A parser which parses a separator to be interleaved with the current parser.</param>
    /// <returns>A parser which applies the current parser at least once, interleaved by <paramref name="separator"/>.</returns>
    public Parser<TToken, IEnumerable<T>> SeparatedAtLeastOnce<Tu>(Parser<TToken, Tu> separator)
    {
		ArgumentNullException.ThrowIfNull(separator);

		return new SeparatedAtLeastOnceParser<TToken, T, Tu>(this, separator);
    }

    /// <summary>
    /// Creates a parser which applies the current parser repeatedly, interleaved and terminated with a specified parser.
    /// The resulting parser ignores the return value of the separator parser.
    /// </summary>
    /// <typeparam name="Tu">The return type of the separator parser.</typeparam>
    /// <param name="separator">A parser which parses a separator to be interleaved with the current parser.</param>
    /// <returns>A parser which applies the current parser repeatedly, interleaved and terminated by <paramref name="separator"/>.</returns>
    public Parser<TToken, IEnumerable<T>> SeparatedAndTerminated<Tu>(Parser<TToken, Tu> separator)
    {
		ArgumentNullException.ThrowIfNull(separator);

		return Before(separator).Many();
    }

    /// <summary>
    /// Creates a parser which applies the current parser at least once, interleaved and terminated with a specified parser.
    /// The resulting parser ignores the return value of the separator parser.
    /// </summary>
    /// <typeparam name="Tu">The return type of the separator parser.</typeparam>
    /// <param name="separator">A parser which parses a separator to be interleaved with the current parser.</param>
    /// <returns>A parser which applies the current parser at least once, interleaved and terminated by <paramref name="separator"/>.</returns>
    public Parser<TToken, IEnumerable<T>> SeparatedAndTerminatedAtLeastOnce<Tu>(Parser<TToken, Tu> separator)
    {
		ArgumentNullException.ThrowIfNull(separator);

		return Before(separator).AtLeastOnce();
    }

    /// <summary>
    /// Creates a parser which applies the current parser repeatedly, interleaved and optionally terminated with a specified parser.
    /// The resulting parser ignores the return value of the separator parser.
    /// </summary>
    /// <typeparam name="Tu">The return type of the separator parser.</typeparam>
    /// <param name="separator">A parser which parses a separator to be interleaved with the current parser.</param>
    /// <returns>A parser which applies the current parser repeatedly, interleaved and optionally terminated by <paramref name="separator"/>.</returns>
    public Parser<TToken, IEnumerable<T>> SeparatedAndOptionallyTerminated<Tu>(Parser<TToken, Tu> separator)
    {
        ArgumentNullException.ThrowIfNull(separator);

        return SeparatedAndOptionallyTerminatedAtLeastOnce(separator)
            .Or(ReturnEmptyEnumerable);
    }

    /// <summary>
    /// Creates a parser which applies the current parser at least once, interleaved and optionally terminated with a specified parser.
    /// The resulting parser ignores the return value of the separator parser.
    /// </summary>
    /// <typeparam name="Tu">The return type of the separator parser.</typeparam>
    /// <param name="separator">A parser which parses a separator to be interleaved with the current parser.</param>
    /// <returns>A parser which applies the current parser at least once, interleaved and optionally terminated by <paramref name="separator"/>.</returns>
    public Parser<TToken, IEnumerable<T>> SeparatedAndOptionallyTerminatedAtLeastOnce<Tu>(Parser<TToken, Tu> separator)
    {
		ArgumentNullException.ThrowIfNull(separator);

		return new SeparatedAndOptionallyTerminatedAtLeastOnceParser<TToken, T, Tu>(this, separator);
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:FileMayOnlyContainASingleType",
    Justification = "This class belongs next to the accompanying API method"
)]
internal sealed class SeparatedAtLeastOnceParser<TToken, T, U>(Parser<TToken, T> parser, Parser<TToken, U> separator)
    : Parser<TToken, IEnumerable<T>>
{
    private readonly Parser<TToken, T> _remainderParser = separator.Then(parser);

    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, [MaybeNullWhen(false)] out IEnumerable<T> result)
    {
        if (!parser.TryParse(ref state, ref expected, out var result1))
        {
            // state.Error set by _parser
            result = null;
            return false;
        }

        var list = new List<T> { result1 };
        if (!Rest(_remainderParser, ref state, ref expected, list))
        {
            result = null;
            return false;
        }

        result = list;
        return true;
    }

    private static bool Rest(Parser<TToken, T> parser, ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expecteds, List<T> ts)
    {
        var lastStartingLoc = state.Location;
        var childExpecteds = new PooledList<Expected<TToken>>(state.Configuration.ArrayPoolProvider.GetArrayPool<Expected<TToken>>());
        while (parser.TryParse(ref state, ref childExpecteds, out var result))
        {
            var endingLoc = state.Location;
            childExpecteds.Clear();

            if (endingLoc <= lastStartingLoc)
            {
                childExpecteds.Dispose();
                throw new InvalidOperationException("Many() used with a parser which consumed no input");
            }

            ts.Add(result);

            lastStartingLoc = endingLoc;
        }

        var lastParserConsumedInput = state.Location > lastStartingLoc;
        if (lastParserConsumedInput)
        {
            expecteds.AddRange(childExpecteds.AsSpan());
        }

        childExpecteds.Dispose();

        // we fail if the most recent parser failed after consuming input.
        // it sets state.Error for us
        return !lastParserConsumedInput;
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:FileMayOnlyContainASingleType",
    Justification = "This class belongs next to the accompanying API method"
)]
internal sealed class SeparatedAndOptionallyTerminatedAtLeastOnceParser<TToken, T, U>(
    Parser<TToken, T> parser,
    Parser<TToken, U> separator) : Parser<TToken, IEnumerable<T>>
{
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, [MaybeNullWhen(false)] out IEnumerable<T> result)
    {
        if (!parser.TryParse(ref state, ref expected, out var result1))
        {
            // state.Error set by _parser
            result = null;
            return false;
        }

        var ts = new List<T> { result1 };

        var childExpecteds = new PooledList<Expected<TToken>>(state.Configuration.ArrayPoolProvider.GetArrayPool<Expected<TToken>>());
        while (true)
        {
            var sepStartLoc = state.Location;
            var sepSuccess = separator.TryParse(ref state, ref childExpecteds, out var _);
            var sepConsumedInput = state.Location > sepStartLoc;

            if (!sepSuccess && sepConsumedInput)
            {
                expected.AddRange(childExpecteds.AsSpan());
            }

            childExpecteds.Clear();

            if (!sepSuccess)
            {
                childExpecteds.Dispose();
                if (sepConsumedInput)
                {
                    // state.Error set by _separator
                    result = null;
                    return false;
                }

                result = ts;
                return true;
            }

            var itemStartLoc = state.Location;
            var itemSuccess = parser.TryParse(ref state, ref childExpecteds, out var itemResult);
            var itemConsumedInput = state.Location > itemStartLoc;

            if (!itemSuccess && itemConsumedInput)
            {
                expected.AddRange(childExpecteds.AsSpan());
            }

            childExpecteds.Clear();

            if (!itemSuccess)
            {
                childExpecteds.Dispose();
                if (itemConsumedInput)
                {
                    // state.Error set by _parser
                    result = null;
                    return false;
                }

                result = ts;
                return true;
            }

            ts.Add(itemResult!);
        }
    }
}
