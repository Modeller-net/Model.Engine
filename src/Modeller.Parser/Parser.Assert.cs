namespace Modeller;

public abstract partial class Parser<TToken, T>
{
    /// <summary>
    /// Creates a parser that fails if the value returned by the current parser fails to satisfy a predicate.
    /// </summary>
    /// <param name="predicate">The predicate to apply to the value returned by the current parser.</param>
    /// <returns>A parser that fails if the value returned by the current parser fails to satisfy <paramref name="predicate"/>.</returns>
    public Parser<TToken, T> Assert(Func<T, bool> predicate)
    {
		ArgumentNullException.ThrowIfNull(predicate);

		return Assert(predicate, "Assertion failed");
    }

    /// <summary>
    /// Creates a parser that fails if the value returned by the current parser fails to satisfy a predicate.
    /// </summary>
    /// <param name="predicate">The predicate to apply to the value returned by the current parser.</param>
    /// <param name="message">A custom error message to return when the value returned by the current parser fails to satisfy the predicate.</param>
    /// <returns>A parser that fails if the value returned by the current parser fails to satisfy <paramref name="predicate"/>.</returns>
    public Parser<TToken, T> Assert(Func<T, bool> predicate, string message)
    {
		ArgumentNullException.ThrowIfNull(predicate);
		ArgumentNullException.ThrowIfNull(message);

		return Assert(predicate, _ => message);
    }

    /// <summary>
    /// Creates a parser that fails if the value returned by the current parser fails to satisfy a predicate.
    /// </summary>
    /// <param name="predicate">The predicate to apply to the value returned by the current parser.</param>
    /// <param name="message">A function to produce a custom error message to return when the value returned by the current parser fails to satisfy the predicate.</param>
    /// <returns>A parser that fails if the value returned by the current parser fails to satisfy <paramref name="predicate"/>.</returns>
    public Parser<TToken, T> Assert(Func<T, bool> predicate, Func<T, string> message)
    {
		ArgumentNullException.ThrowIfNull(predicate);
		ArgumentNullException.ThrowIfNull(message);

		return new AssertParser<TToken, T>(this, predicate, message);
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:FileMayOnlyContainASingleType",
    Justification = "This class belongs next to the accompanying API method"
)]
internal sealed class AssertParser<TToken, T>(
    Parser<TToken, T> parser,
    Func<T, bool> predicate,
    Func<T, string> message)
    : Parser<TToken, T>
{
    private static readonly Expected<TToken> s_expected
        = new("result satisfying assertion");

    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, [MaybeNullWhen(false)] out T result)
    {
        var childExpected = new PooledList<Expected<TToken>>(state.Configuration.ArrayPoolProvider.GetArrayPool<Expected<TToken>>());

        var success = parser.TryParse(ref state, ref childExpected, out result);

        if (success)
        {
            expected.AddRange(childExpected.AsSpan());
        }

        childExpected.Dispose();

        if (!success)
        {
            return false;
        }

        // result is not null hereafter
        if (!predicate(result!))
        {
            state.SetError(Maybe.Nothing<TToken>(), false, state.Location, message(result!));
            expected.Add(s_expected);

            result = default;
            return false;
        }

#pragma warning disable CS8762  // Parameter 'result' must have a non-null value when exiting with 'true'.
        return true;
#pragma warning restore CS8762
    }
}
