namespace Modeller;

public abstract partial class Parser<TToken, T>
{
    /// <summary>
    /// Creates a parser which applies the current parser followed by a specified parser.
    /// The resulting parser returns the result of the second parser, ignoring the result of the current parser.
    /// </summary>
    /// <typeparam name="Tu">The return type of the second parser.</typeparam>
    /// <param name="parser">A parser to apply after applying the current parser.</param>
    /// <returns>A parser which applies the current parser followed by <paramref name="parser"/>.</returns>
    public Parser<TToken, Tu> Then<Tu>(Parser<TToken, Tu> parser)
    {
		ArgumentNullException.ThrowIfNull(parser);

		return Then(parser, (t, u) => u);
    }

    /// <summary>
    /// Creates a parser which applies the current parser followed by a specified parser, applying a function to the two results.
    /// </summary>
    /// <remarks>
    /// This is a synonym for <see cref="Parser.Map{TToken, T1, T2, R}(Func{T1, T2, R}, Parser{TToken, T1}, Parser{TToken, T2})"/>
    /// with the arguments rearranged.
    /// </remarks>
    /// <typeparam name="Tu">The return type of the second parser.</typeparam>
    /// <typeparam name="Tr">The return type of the composed parser.</typeparam>
    /// <param name="parser">A parser to apply after applying the current parser.</param>
    /// <param name="result">A function to apply to the two parsed values.</param>
    /// <returns>A parser which applies the current parser followed by <paramref name="parser"/>.</returns>
    public Parser<TToken, Tr> Then<Tu, Tr>(Parser<TToken, Tu> parser, Func<T, Tu, Tr> result)
    {
		ArgumentNullException.ThrowIfNull(parser);
		ArgumentNullException.ThrowIfNull(result);

		return Parser.Map(result, this, parser);
    }

    /// <summary>
    /// Creates a parser that applies a transformation function to the return value of the current parser.
    /// The transformation function dynamically chooses a second parser, which is applied after applying the current parser.
    /// </summary>
    /// <remarks>This function is a synonym for <see cref="Parser{TToken, T}.Bind{U}(Func{T, Parser{TToken, U}})"/>.</remarks>
    /// <param name="selector">A transformation function which returns a parser to apply after applying the current parser.</param>
    /// <typeparam name="Tu">The type of the return value of the second parser.</typeparam>
    /// <returns>A parser which applies the current parser before applying the result of the <paramref name="selector"/> function.</returns>
    public Parser<TToken, Tu> Then<Tu>(Func<T, Parser<TToken, Tu>> selector)
    {
		ArgumentNullException.ThrowIfNull(selector);

		return Bind(selector);
    }

    /// <summary>
    /// Creates a parser that applies a transformation function to the return value of the current parser.
    /// The transformation function dynamically chooses a second parser, which is applied after applying the current parser.
    /// </summary>
    /// <remarks>This function is a synonym for <see cref="Parser{TToken, T}.Bind{U, R}(Func{T, Parser{TToken, U}}, Func{T, U, R})"/>.</remarks>
    /// <param name="selector">A transformation function which returns a parser to apply after applying the current parser.</param>
    /// <param name="result">A function to apply to the return values of the two parsers.</param>
    /// <typeparam name="Tu">The type of the return value of the second parser.</typeparam>
    /// <typeparam name="Tr">The type of the return value of the resulting parser.</typeparam>
    /// <returns>A parser which applies the current parser before applying the result of the <paramref name="selector"/> function.</returns>
    public Parser<TToken, Tr> Then<Tu, Tr>(Func<T, Parser<TToken, Tu>> selector, Func<T, Tu, Tr> result)
    {
		ArgumentNullException.ThrowIfNull(selector);
		ArgumentNullException.ThrowIfNull(result);

		return Bind(selector, result);
    }
}
