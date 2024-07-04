namespace Modeller;

public abstract partial class Parser<TToken, T>
{
    /// <summary>
    /// Creates a parser which applies this parser zero or more times
    /// until <paramref name="terminator"/> succeeds.
    /// Fails if this parser fails or if <paramref name="terminator"/>
    /// fails after consuming input.
    /// The return value of <paramref name="terminator"/> is ignored.
    /// </summary>
    /// <remarks>
    /// <c>p.Until(q)</c> is equivalent to
    /// <c>p.ManyThen(q).Select(t => t.Item1)</c>.
    /// </remarks>
    /// <typeparam name="Tu">
    /// The return type of <paramref name="terminator"/>.
    /// </typeparam>
    /// <param name="terminator">A parser to parse a terminator.</param>
    /// <returns>A parser which applies this parser repeatedly until <paramref name="terminator"/> succeeds.</returns>
    public Parser<TToken, IEnumerable<T>> Until<Tu>(Parser<TToken, Tu> terminator)
    {
		ArgumentNullException.ThrowIfNull(terminator);

		return ManyThen(terminator).Select((Func<(IEnumerable<T>, Tu), IEnumerable<T>>)(tup => tup.Item1));
    }

    /// <summary>
    /// Creates a parser which applies this parser zero or more times
    /// until <paramref name="terminator"/> succeeds.
    /// Fails if this parser fails or if <paramref name="terminator"/>
    /// fails after consuming input.
    /// </summary>
    /// <typeparam name="Tu">The return type of <paramref name="terminator"/>.</typeparam>
    /// <param name="terminator">A parser to parse a terminator.</param>
    /// <returns>A parser which applies this parser repeatedly until <paramref name="terminator"/> succeeds.</returns>
    public Parser<TToken, (IEnumerable<T>, Tu)> ManyThen<Tu>(Parser<TToken, Tu> terminator)
    {
		ArgumentNullException.ThrowIfNull(terminator);

		return terminator.Select(t => (Enumerable.Empty<T>(), t))
            .Or(AtLeastOnceThen(terminator));
    }

    /// <summary>
    /// Creates a parser which applies this parser one or more times until
    /// <paramref name="terminator"/> succeeds.
    /// Fails if this parser fails or if <paramref name="terminator"/>
    /// fails after consuming input.
    /// The return value of <paramref name="terminator"/> is ignored.
    /// </summary>
    /// <remarks>
    /// <c>p.AtLeastOnceUntil(q)</c> is equivalent to
    /// <c>p.AtLeastOnceThen(q).Select(t => t.Item1)</c>.
    /// </remarks>
    /// <param name="terminator">A parser to parse a terminator.</param>
    /// <typeparam name="Tu">The return type of the <paramref name="terminator"/> parser.</typeparam>
    /// <returns>
    /// A parser which applies this parser repeatedly
    /// until <paramref name="terminator"/> succeeds.
    /// </returns>
    public Parser<TToken, IEnumerable<T>> AtLeastOnceUntil<Tu>(Parser<TToken, Tu> terminator)
    {
		ArgumentNullException.ThrowIfNull(terminator);

		return AtLeastOnceThen(terminator).Select(new Func<(IEnumerable<T>, Tu), IEnumerable<T>>(tup => tup.Item1));
    }

    /// <summary>
    /// Creates a parser which applies this parser one or more times
    /// until <paramref name="terminator"/> succeeds.
    /// Fails if this parser fails or if <paramref name="terminator"/>
    /// fails after consuming input.
    /// </summary>
    /// <param name="terminator">A parser to parse a terminator.</param>
    /// <typeparam name="Tu">The return type of the <paramref name="terminator"/> parser.</typeparam>
    /// <returns>
    /// A parser which applies this parser repeatedly
    /// until <paramref name="terminator"/> succeeds.
    /// </returns>
    public Parser<TToken, (IEnumerable<T>, Tu)> AtLeastOnceThen<Tu>(Parser<TToken, Tu> terminator)
    {
		ArgumentNullException.ThrowIfNull(terminator);

		return new AtLeastOnceThenParser<TToken, T, Tu>(this, terminator, true)!;
    }

    /// <summary>
    /// Creates a parser which applies this parser zero or more times
    /// until <paramref name="terminator"/> succeeds,
    /// discarding the results. This is more efficient than
    /// <see cref="Until{U}(Parser{TToken, U})"/> if you don't
    /// need the results.
    /// Fails if this parser fails or if <paramref name="terminator"/>
    /// fails after consuming input.
    /// The return value of <paramref name="terminator"/> is ignored.
    /// </summary>
    /// <remarks>
    /// <c>p.SkipUntil(q)</c> is equivalent to
    /// <c>p.SkipManyThen(q).ThenReturn(Unit.Value)</c>.
    /// </remarks>
    /// <typeparam name="Tu">The return type of <paramref name="terminator"/>.</typeparam>
    /// <param name="terminator">A parser to parse a terminator.</param>
    /// <returns>A parser which applies this parser repeatedly until <paramref name="terminator"/> succeeds, discarding the results.</returns>
    public Parser<TToken, Unit> SkipUntil<Tu>(Parser<TToken, Tu> terminator)
    {
		ArgumentNullException.ThrowIfNull(terminator);

		return SkipManyThen(terminator).Then(ReturnUnit);
    }

    /// <summary>
    /// Creates a parser which applies this parser zero or more times
    /// until <paramref name="terminator"/> succeeds,
    /// discarding the results. This is more efficient than
    /// <see cref="ManyThen{U}(Parser{TToken, U})"/> if you don't
    /// need the results.
    /// Fails if this parser fails or if <paramref name="terminator"/>
    /// fails after consuming input.
    /// </summary>
    /// <typeparam name="Tu">The return type of <paramref name="terminator"/>.</typeparam>
    /// <param name="terminator">A parser to parse a terminator.</param>
    /// <returns>
    /// A parser which applies this parser repeatedly until
    /// <paramref name="terminator"/> succeeds, discarding the results.
    /// </returns>
    public Parser<TToken, Tu> SkipManyThen<Tu>(Parser<TToken, Tu> terminator)
    {
		ArgumentNullException.ThrowIfNull(terminator);

		return terminator.Or(SkipAtLeastOnceThen(terminator));
    }

    /// <summary>
    /// Creates a parser which applies this parser one or more times
    /// until <paramref name="terminator"/> succeeds,
    /// discarding the results. This is more efficient than
    /// <see cref="AtLeastOnceUntil{U}(Parser{TToken, U})"/>
    /// if you don't need the results.
    /// Fails if this parser fails or if <paramref name="terminator"/>
    /// fails after consuming input.
    /// The return value of <paramref name="terminator"/> is ignored.
    /// </summary>
    /// <remarks>
    /// <c>p.SkipAtLeastOnceUntil(q)</c> is equivalent to
    /// <c>p.SkipAtLeastOnceThen(q).ThenReturn(Unit.Value)</c>.
    /// </remarks>
    /// <param name="terminator">A parser to parse a terminator.</param>
    /// <typeparam name="Tu">The return type of the <paramref name="terminator"/> parser.</typeparam>
    /// <returns>
    /// A parser which applies this parser repeatedly until
    /// <paramref name="terminator"/> succeeds, discarding the results.
    /// </returns>
    public Parser<TToken, Unit> SkipAtLeastOnceUntil<Tu>(Parser<TToken, Tu> terminator)
    {
		ArgumentNullException.ThrowIfNull(terminator);

		return SkipAtLeastOnceThen(terminator).Then(ReturnUnit);
    }

    /// <summary>
    /// Creates a parser which applies this parser one or more times
    /// until <paramref name="terminator"/> succeeds,
    /// discarding the results. This is more efficient than
    /// <see cref="AtLeastOnceThen{U}(Parser{TToken, U})"/>
    /// if you don't need the results. Fails if this parser fails or if
    /// <paramref name="terminator"/> fails after consuming input.
    /// </summary>
    /// <param name="terminator">A parser to parse a terminator.</param>
    /// <typeparam name="Tu">The return type of the <paramref name="terminator"/> parser.</typeparam>
    /// <returns>
    /// A parser which applies this parser repeatedly until
    /// <paramref name="terminator"/> succeeds, discarding the results.
    /// </returns>
    public Parser<TToken, Tu> SkipAtLeastOnceThen<Tu>(Parser<TToken, Tu> terminator)
    {
		ArgumentNullException.ThrowIfNull(terminator);

		return new AtLeastOnceThenParser<TToken, T, Tu>(this, terminator, false).Select(tup => tup.Item2);
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:FileMayOnlyContainASingleType",
    Justification = "This class belongs next to the accompanying API method"
)]
public sealed class AtLeastOnceThenParser<TToken, T, Tu>(
    Parser<TToken, T> parser,
    Parser<TToken, Tu> terminator,
    bool keepResults)
    : Parser<TToken, (IEnumerable<T>?, Tu)>
{
    // see comment about expected in ParseState.Error.cs
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, [MaybeNullWhen(false)] out (IEnumerable<T>?, Tu) result)
    {
        var ts = keepResults ? new List<T>() : null;

        var firstItemStartLoc = state.Location;

        if (!parser.TryParse(ref state, ref expected, out var result1))
        {
            // state.Error set by _parser
            result = (null, default!);
            return false;
        }

        if (state.Location <= firstItemStartLoc)
        {
            throw new InvalidOperationException("Until() used with a parser which consumed no input");
        }

        ts?.Add(result1);

        var terminatorExpecteds = new PooledList<Expected<TToken>>(state.Configuration.ArrayPoolProvider.GetArrayPool<Expected<TToken>>());
        var itemExpecteds = new PooledList<Expected<TToken>>(state.Configuration.ArrayPoolProvider.GetArrayPool<Expected<TToken>>());
        while (true)
        {
            var terminatorStartLoc = state.Location;
            var terminatorSuccess = terminator.TryParse(ref state, ref terminatorExpecteds, out var terminatorResult);
            if (terminatorSuccess)
            {
                terminatorExpecteds.Dispose();
                itemExpecteds.Dispose();
                result = (ts!, terminatorResult!);
                return true;
            }

            if (state.Location > terminatorStartLoc)
            {
                // state.Error set by _terminator
                expected.AddRange(terminatorExpecteds.AsSpan());
                terminatorExpecteds.Dispose();
                itemExpecteds.Dispose();
                result = (null, default!);
                return false;
            }

            var itemStartLoc = state.Location;
            var itemSuccess = parser.TryParse(ref state, ref itemExpecteds, out var itemResult);
            var itemConsumedInput = state.Location > itemStartLoc;
            if (!itemSuccess)
            {
                if (!itemConsumedInput)
                {
                    // get the expected from both _terminator and _parser
                    expected.AddRange(terminatorExpecteds.AsSpan());
                    expected.AddRange(itemExpecteds.AsSpan());
                }
                else
                {
                    // throw out the _terminator expecteds and keep only _parser
                    expected.AddRange(itemExpecteds.AsSpan());
                }

                terminatorExpecteds.Dispose();
                itemExpecteds.Dispose();
                result = (null, default!);
                return false;
            }

            // throw out both sets of expecteds
            terminatorExpecteds.Clear();
            itemExpecteds.Clear();
            if (!itemConsumedInput)
            {
                throw new InvalidOperationException("Until() used with a parser which consumed no input");
            }

            ts?.Add(itemResult!);
        }
    }
}
