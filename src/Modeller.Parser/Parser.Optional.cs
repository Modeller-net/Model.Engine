namespace Modeller;

public abstract partial class Parser<TToken, T>
{
	private static Parser<TToken, Maybe<T>>? s_returnNothing;

	private static Parser<TToken, Maybe<T>> ReturnNothing
	{
		get
		{
			if (s_returnNothing == null)
			{
				s_returnNothing = Parser<TToken>.Return(Maybe.Nothing<T>());
			}

			return s_returnNothing;
		}
	}

	/// <summary>
	/// Creates a parser which applies the current parser and returns <see cref="Maybe.Nothing{T}()"/> if the current parser fails without consuming any input.
	/// The resulting parser fails if the current parser fails after consuming input.
	/// </summary>
	/// <returns>A parser which applies the current parser and returns <see cref="Maybe.Nothing{T}()"/> if the current parser fails without consuming any input.</returns>
	public Parser<TToken, Maybe<T>> Optional()
		=> Select(Maybe.Just).Or(ReturnNothing);
}
