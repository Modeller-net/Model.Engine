namespace Modeller;


public abstract partial class Parser<TToken, T>
{
    /// <summary>
    /// Creates a parser that applies a transformation function to the return value of the current parser.
    /// The transformation function dynamically chooses a second parser, which is applied after applying the current parser.
    /// </summary>
    /// <param name="selector">A transformation function which returns a parser to apply after applying the current parser.</param>
    /// <typeparam name="Tu">The type of the return value of the second parser.</typeparam>
    /// <returns>A parser which applies the current parser before applying the result of the <paramref name="selector"/> function.</returns>
    public Parser<TToken, Tu> Bind<Tu>(Func<T, Parser<TToken, Tu>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return Bind(selector, (t, u) => u);
    }

    /// <summary>
    /// Creates a parser that applies a transformation function to the return value of the current parser.
    /// The transformation function dynamically chooses a second parser, which is applied after applying the current parser.
    /// </summary>
    /// <param name="selector">A transformation function which returns a parser to apply after applying the current parser.</param>
    /// <param name="result">A function to apply to the return values of the two parsers.</param>
    /// <typeparam name="Tu">The type of the return value of the second parser.</typeparam>
    /// <typeparam name="Tr">The type of the return value of the resulting parser.</typeparam>
    /// <returns>A parser which applies the current parser before applying the result of the <paramref name="selector"/> function.</returns>
    public Parser<TToken, Tr> Bind<Tu, Tr>(Func<T, Parser<TToken, Tu>> selector, Func<T, Tu, Tr> result)
    {
		ArgumentNullException.ThrowIfNull(selector);
		ArgumentNullException.ThrowIfNull(result);

		return new BindParser<TToken, T, Tu, Tr>(this, selector, result);
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:FileMayOnlyContainASingleType",
    Justification = "This class belongs next to the accompanying API method"
)]
internal sealed class BindParser<TToken, T, Tu, Tr>(
    Parser<TToken, T> parser,
    Func<T, Parser<TToken, Tu>> func,
    Func<T, Tu, Tr> result)
    : Parser<TToken, Tr>
{
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, [MaybeNullWhen(false)] out Tr result1)
    {
        var success = parser.TryParse(ref state, ref expected, out var childResult);
        if (!success)
        {
            // state.Error set by _parser
            result1 = default;
            return false;
        }

        var nextParser = func(childResult!);
        var nextSuccess = nextParser.TryParse(ref state, ref expected, out var nextResult);

        if (!nextSuccess)
        {
            // state.Error set by nextParser
            result1 = default;
            return false;
        }

        result1 = result(childResult!, nextResult!);
        return true;
    }
}
