namespace Modeller;

public partial class Parser<TToken, T>
{
    /// <summary>
    /// Creates a parser equivalent to the current parser, with a custom label.
    /// The label will be reported in an error message if the parser fails, instead of the default error message.
    /// <seealso cref="ParseError{TToken}.Expected"/>
    /// <seealso cref="Expected{TToken}.Label"/>
    /// </summary>
    /// <param name="label">The custom label to apply to the current parser.</param>
    /// <returns>A parser equivalent to the current parser, with a custom label.</returns>
    public Parser<TToken, T> Labelled(string label)
    {
		ArgumentNullException.ThrowIfNull(label);

		return WithExpected([new(label)]);
    }

    internal Parser<TToken, T> WithExpected(ImmutableArray<Expected<TToken>> expected)
        => new WithExpectedParser<TToken, T>(this, expected);
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:FileMayOnlyContainASingleType",
    Justification = "This class belongs next to the accompanying API method"
)]
internal sealed class WithExpectedParser<TToken, T>(Parser<TToken, T> parser, ImmutableArray<Expected<TToken>> expected)
    : Parser<TToken, T>
{
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected1, [MaybeNullWhen(false)] out T result)
    {
        var childExpecteds = new PooledList<Expected<TToken>>(state.Configuration.ArrayPoolProvider.GetArrayPool<Expected<TToken>>());
        var success = parser.TryParse(ref state, ref childExpecteds, out result);
        if (!success)
        {
            expected1.AddRange(expected);
        }

        childExpecteds.Dispose();

        // result is not null here
        return success;
    }
}
