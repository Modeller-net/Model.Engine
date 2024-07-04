namespace Modeller;

public abstract partial class Parser<TToken, T>
{
	/// <summary>
	/// Creates a parser which applies the specified transformation function to the result of the current parser.
	/// This is an infix synonym for <see cref="Parser.Map{TToken, T1, R}(Func{T1, R}, Parser{TToken, T1})"/>.
	/// </summary>
	/// <typeparam name="Tu">The return type of the transformation function.</typeparam>
	/// <param name="selector">A transformation function.</param>
	/// <returns>A parser which applies <paramref name="selector"/> to the result of the current parser.</returns>
	public Parser<TToken, Tu> Select<Tu>(Func<T, Tu> selector)
	{
        ArgumentNullException.ThrowIfNull(selector);

        return Parser.Map(selector, this);
	}

	/// <summary>
	/// Creates a parser which applies the specified transformation function to the result of the current parser.
	/// This is an infix synonym for <see cref="Parser.Map{TToken, T1, R}(Func{T1, R}, Parser{TToken, T1})"/>.
	/// </summary>
	/// <typeparam name="Tu">The return type of the transformation function.</typeparam>
	/// <param name="selector">A transformation function.</param>
	/// <returns>A parser which applies <paramref name="selector"/> to the result of the current parser.</returns>
	public Parser<TToken, Tu> Map<Tu>(Func<T, Tu> selector)
	{
        ArgumentNullException.ThrowIfNull(selector);

        return Parser.Map(selector, this);
	}
}
