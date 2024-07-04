namespace Modeller;

public abstract partial class Parser<TToken, T>
{
	/// <summary>
	/// Creates a parser which casts the return value of the current parser to the specified result type, or fails if the return value is not an instance of <typeparamref name="Tu"/>.
	/// </summary>
	/// <typeparam name="Tu">The type to cast the return value of the current parser to.</typeparam>
	/// <returns>A parser which returns the current parser's return value casted to <typeparamref name="Tu"/>, if the value is an instance of <typeparamref name="Tu"/>.</returns>
	public Parser<TToken, Tu> OfType<Tu>()
		=>
			Assert(x => x is Tu, x => $"Expected a {typeof(Tu).Name} but got a {x!.GetType().Name}")
				.Cast<Tu>()
				.Labelled($"result of type {typeof(Tu).Name}");
}
