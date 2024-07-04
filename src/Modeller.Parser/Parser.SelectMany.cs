namespace Modeller;

public abstract partial class Parser<TToken, T>
{
	/// <summary>
	/// Creates a parser that applies a transformation function to the return value of the current parser.
	/// The transformation function dynamically chooses a second parser, which is applied after applying the current parser.
	/// </summary>
	/// <param name="selector">A transformation function which returns a parser to apply after applying the current parser.</param>
	/// <param name="result">A function to apply to the return values of the two parsers.</param>
	/// <typeparam name="Tu">The type of the return value of the second parser.</typeparam>
	/// <typeparam name="Tr">The type of the return value of the resulting parser.</typeparam>
	/// <returns>A parser which applies the current parser before applying the result of the <paramref name="selector"/> function.</returns>
	public Parser<TToken, Tr> SelectMany<Tu, Tr>(Func<T, Parser<TToken, Tu>> selector, Func<T, Tu, Tr> result)
	{
		ArgumentNullException.ThrowIfNull(selector);
		ArgumentNullException.ThrowIfNull(result);

		return Bind(selector, result);
	}
}
