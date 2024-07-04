namespace Modeller;

public abstract partial class Parser<TToken, T>
{
	/// <summary>
	/// Cast the return value of the current parser to the specified result type.
	/// </summary>
	/// <typeparam name="Tu">The type to cast the return value to.</typeparam>
	/// <exception cref="System.InvalidCastException">Thrown when the return value is not an instance of <typeparamref name="Tu"/>.</exception>
	/// <returns>A parser which returns this parser's return value casted to <typeparamref name="Tu"/>.</returns>
	public Parser<TToken, Tu> Cast<Tu>() => Select(x => (Tu)(object)x!);
}
