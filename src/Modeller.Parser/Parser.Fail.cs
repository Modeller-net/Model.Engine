namespace Modeller;

public static partial class Parser<TToken>
{
	/// <summary>
	/// Creates a parser which always fails without consuming any input.
	/// </summary>
	/// <param name="message">A custom error message.</param>
	/// <typeparam name="T">The return type of the resulting parser.</typeparam>
	/// <returns>A parser which always fails.</returns>
	public static Parser<TToken, T> Fail<T>(string message = "Failed")
	{
        ArgumentNullException.ThrowIfNull(message);

        return new FailParser<TToken, T>(message);
	}
}

[SuppressMessage(
	"StyleCop.CSharp.MaintainabilityRules",
	"SA1402:FileMayOnlyContainASingleType",
	Justification = "This class belongs next to the accompanying API method"
)]
internal sealed class FailParser<TToken, T>(string message) : Parser<TToken, T>
{
	private static readonly Expected<TToken> s_expected
		= new(ImmutableArray<TToken>.Empty);

    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, [MaybeNullWhen(false)] out T result)
	{
		state.SetError(Maybe.Nothing<TToken>(), false, state.Location, message);
		expected.Add(s_expected);
		result = default;
		return false;
	}
}
