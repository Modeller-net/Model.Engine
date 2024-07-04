namespace Modeller;

public static partial class Parser
{
	/// <summary>
	/// Creates a parser that applies the specified parsers sequentially and applies the specified transformation function to their results.
	/// </summary>
	/// <param name="func">A function to apply to the return values of the specified parsers</param>
	/// <param name="parser1">The first parser</param>
	/// <typeparam name="TToken">The type of tokens in the parser's input stream</typeparam>
	/// <typeparam name="T1">The return type of the first parser</typeparam>
	/// <typeparam name="Tr">The return type of the resulting parser</typeparam>
	public static Parser<TToken, Tr> Map<TToken, T1, Tr>(
		Func<T1, Tr> func,
		Parser<TToken, T1> parser1
	)
	{
		ArgumentNullException.ThrowIfNull(func);
		ArgumentNullException.ThrowIfNull(parser1);

		return parser1 is MapParserBase<TToken, T1> p
			? p.Map(func)
			: new Map1Parser<TToken, T1, Tr>(func, parser1);
	}

	/// <summary>
	/// Creates a parser that applies the specified parsers sequentially and applies the specified transformation function to their results.
	/// </summary>
	/// <param name="func">A function to apply to the return values of the specified parsers</param>
	/// <param name="parser1">The first parser</param>
	/// <param name="parser2">The second parser</param>
	/// <typeparam name="TToken">The type of tokens in the parser's input stream</typeparam>
	/// <typeparam name="T1">The return type of the first parser</typeparam>
	///<typeparam name="T2">The return type of the second parser</typeparam>
	/// <typeparam name="Tr">The return type of the resulting parser</typeparam>
	public static Parser<TToken, Tr> Map<TToken, T1, T2, Tr>(
		Func<T1, T2, Tr> func,
		Parser<TToken, T1> parser1,
		Parser<TToken, T2> parser2
	)
	{
		ArgumentNullException.ThrowIfNull(func);
		ArgumentNullException.ThrowIfNull(parser1);
		ArgumentNullException.ThrowIfNull(parser2);

		return new Map2Parser<TToken, T1, T2, Tr>(func, parser1, parser2);
	}

	/// <summary>
	/// Creates a parser that applies the specified parsers sequentially and applies the specified transformation function to their results.
	/// </summary>
	/// <param name="func">A function to apply to the return values of the specified parsers</param>
	/// <param name="parser1">The first parser</param>
	/// <param name="parser2">The second parser</param>
	/// <param name="parser3">The third parser</param>
	/// <typeparam name="TToken">The type of tokens in the parser's input stream</typeparam>
	/// <typeparam name="T1">The return type of the first parser</typeparam>
	///<typeparam name="T2">The return type of the second parser</typeparam>
	///<typeparam name="T3">The return type of the third parser</typeparam>
	/// <typeparam name="Tr">The return type of the resulting parser</typeparam>
	public static Parser<TToken, Tr> Map<TToken, T1, T2, T3, Tr>(
		Func<T1, T2, T3, Tr> func,
		Parser<TToken, T1> parser1,
		Parser<TToken, T2> parser2,
		Parser<TToken, T3> parser3
	)
	{
		ArgumentNullException.ThrowIfNull(func);
		ArgumentNullException.ThrowIfNull(parser1);
		ArgumentNullException.ThrowIfNull(parser2);
		ArgumentNullException.ThrowIfNull(parser3);

		return new Map3Parser<TToken, T1, T2, T3, Tr>(func, parser1, parser2, parser3);
	}

	/// <summary>
	/// Creates a parser that applies the specified parsers sequentially and applies the specified transformation function to their results.
	/// </summary>
	/// <param name="func">A function to apply to the return values of the specified parsers</param>
	/// <param name="parser1">The first parser</param>
	/// <param name="parser2">The second parser</param>
	/// <param name="parser3">The third parser</param>
	/// <param name="parser4">The fourth parser</param>
	/// <typeparam name="TToken">The type of tokens in the parser's input stream</typeparam>
	/// <typeparam name="T1">The return type of the first parser</typeparam>
	///<typeparam name="T2">The return type of the second parser</typeparam>
	///<typeparam name="T3">The return type of the third parser</typeparam>
	///<typeparam name="T4">The return type of the fourth parser</typeparam>
	/// <typeparam name="Tr">The return type of the resulting parser</typeparam>
	public static Parser<TToken, Tr> Map<TToken, T1, T2, T3, T4, Tr>(
		Func<T1, T2, T3, T4, Tr> func,
		Parser<TToken, T1> parser1,
		Parser<TToken, T2> parser2,
		Parser<TToken, T3> parser3,
		Parser<TToken, T4> parser4
	)
	{
		ArgumentNullException.ThrowIfNull(func);
		ArgumentNullException.ThrowIfNull(parser1);
		ArgumentNullException.ThrowIfNull(parser2);
		ArgumentNullException.ThrowIfNull(parser3);
		ArgumentNullException.ThrowIfNull(parser4);

		return new Map4Parser<TToken, T1, T2, T3, T4, Tr>(func, parser1, parser2, parser3, parser4);
	}

	/// <summary>
	/// Creates a parser that applies the specified parsers sequentially and applies the specified transformation function to their results.
	/// </summary>
	/// <param name="func">A function to apply to the return values of the specified parsers</param>
	/// <param name="parser1">The first parser</param>
	/// <param name="parser2">The second parser</param>
	/// <param name="parser3">The third parser</param>
	/// <param name="parser4">The fourth parser</param>
	/// <param name="parser5">The fifth parser</param>
	/// <typeparam name="TToken">The type of tokens in the parser's input stream</typeparam>
	/// <typeparam name="T1">The return type of the first parser</typeparam>
	///<typeparam name="T2">The return type of the second parser</typeparam>
	///<typeparam name="T3">The return type of the third parser</typeparam>
	///<typeparam name="T4">The return type of the fourth parser</typeparam>
	///<typeparam name="T5">The return type of the fifth parser</typeparam>
	/// <typeparam name="Tr">The return type of the resulting parser</typeparam>
	public static Parser<TToken, Tr> Map<TToken, T1, T2, T3, T4, T5, Tr>(
		Func<T1, T2, T3, T4, T5, Tr> func,
		Parser<TToken, T1> parser1,
		Parser<TToken, T2> parser2,
		Parser<TToken, T3> parser3,
		Parser<TToken, T4> parser4,
		Parser<TToken, T5> parser5
	)
	{
		ArgumentNullException.ThrowIfNull(func);
		ArgumentNullException.ThrowIfNull(parser1);
		ArgumentNullException.ThrowIfNull(parser2);
		ArgumentNullException.ThrowIfNull(parser3);
		ArgumentNullException.ThrowIfNull(parser4);
		ArgumentNullException.ThrowIfNull(parser5);

		return new Map5Parser<TToken, T1, T2, T3, T4, T5, Tr>(func, parser1, parser2, parser3, parser4, parser5);
	}

	/// <summary>
	/// Creates a parser that applies the specified parsers sequentially and applies the specified transformation function to their results.
	/// </summary>
	/// <param name="func">A function to apply to the return values of the specified parsers</param>
	/// <param name="parser1">The first parser</param>
	/// <param name="parser2">The second parser</param>
	/// <param name="parser3">The third parser</param>
	/// <param name="parser4">The fourth parser</param>
	/// <param name="parser5">The fifth parser</param>
	/// <param name="parser6">The sixth parser</param>
	/// <typeparam name="TToken">The type of tokens in the parser's input stream</typeparam>
	/// <typeparam name="T1">The return type of the first parser</typeparam>
	///<typeparam name="T2">The return type of the second parser</typeparam>
	///<typeparam name="T3">The return type of the third parser</typeparam>
	///<typeparam name="T4">The return type of the fourth parser</typeparam>
	///<typeparam name="T5">The return type of the fifth parser</typeparam>
	///<typeparam name="T6">The return type of the sixth parser</typeparam>
	/// <typeparam name="Tr">The return type of the resulting parser</typeparam>
	public static Parser<TToken, Tr> Map<TToken, T1, T2, T3, T4, T5, T6, Tr>(
		Func<T1, T2, T3, T4, T5, T6, Tr> func,
		Parser<TToken, T1> parser1,
		Parser<TToken, T2> parser2,
		Parser<TToken, T3> parser3,
		Parser<TToken, T4> parser4,
		Parser<TToken, T5> parser5,
		Parser<TToken, T6> parser6
	)
	{
		ArgumentNullException.ThrowIfNull(func);
		ArgumentNullException.ThrowIfNull(parser1);
		ArgumentNullException.ThrowIfNull(parser2);
		ArgumentNullException.ThrowIfNull(parser3);
		ArgumentNullException.ThrowIfNull(parser4);
		ArgumentNullException.ThrowIfNull(parser5);
		ArgumentNullException.ThrowIfNull(parser6);

		return new Map6Parser<TToken, T1, T2, T3, T4, T5, T6, Tr>(func, parser1, parser2, parser3, parser4, parser5, parser6);
	}

	/// <summary>
	/// Creates a parser that applies the specified parsers sequentially and applies the specified transformation function to their results.
	/// </summary>
	/// <param name="func">A function to apply to the return values of the specified parsers</param>
	/// <param name="parser1">The first parser</param>
	/// <param name="parser2">The second parser</param>
	/// <param name="parser3">The third parser</param>
	/// <param name="parser4">The fourth parser</param>
	/// <param name="parser5">The fifth parser</param>
	/// <param name="parser6">The sixth parser</param>
	/// <param name="parser7">The seventh parser</param>
	/// <typeparam name="TToken">The type of tokens in the parser's input stream</typeparam>
	/// <typeparam name="T1">The return type of the first parser</typeparam>
	///<typeparam name="T2">The return type of the second parser</typeparam>
	///<typeparam name="T3">The return type of the third parser</typeparam>
	///<typeparam name="T4">The return type of the fourth parser</typeparam>
	///<typeparam name="T5">The return type of the fifth parser</typeparam>
	///<typeparam name="T6">The return type of the sixth parser</typeparam>
	///<typeparam name="T7">The return type of the seventh parser</typeparam>
	/// <typeparam name="Tr">The return type of the resulting parser</typeparam>
	public static Parser<TToken, Tr> Map<TToken, T1, T2, T3, T4, T5, T6, T7, Tr>(
		Func<T1, T2, T3, T4, T5, T6, T7, Tr> func,
		Parser<TToken, T1> parser1,
		Parser<TToken, T2> parser2,
		Parser<TToken, T3> parser3,
		Parser<TToken, T4> parser4,
		Parser<TToken, T5> parser5,
		Parser<TToken, T6> parser6,
		Parser<TToken, T7> parser7
	)
	{
		ArgumentNullException.ThrowIfNull(func);
		ArgumentNullException.ThrowIfNull(parser1);
		ArgumentNullException.ThrowIfNull(parser2);
		ArgumentNullException.ThrowIfNull(parser3);
		ArgumentNullException.ThrowIfNull(parser4);
		ArgumentNullException.ThrowIfNull(parser5);
		ArgumentNullException.ThrowIfNull(parser6);
		ArgumentNullException.ThrowIfNull(parser7);

		return new Map7Parser<TToken, T1, T2, T3, T4, T5, T6, T7, Tr>(func, parser1, parser2, parser3, parser4, parser5, parser6, parser7);
	}

	/// <summary>
	/// Creates a parser that applies the specified parsers sequentially and applies the specified transformation function to their results.
	/// </summary>
	/// <param name="func">A function to apply to the return values of the specified parsers</param>
	/// <param name="parser1">The first parser</param>
	/// <param name="parser2">The second parser</param>
	/// <param name="parser3">The third parser</param>
	/// <param name="parser4">The fourth parser</param>
	/// <param name="parser5">The fifth parser</param>
	/// <param name="parser6">The sixth parser</param>
	/// <param name="parser7">The seventh parser</param>
	/// <param name="parser8">The eighth parser</param>
	/// <typeparam name="TToken">The type of tokens in the parser's input stream</typeparam>
	/// <typeparam name="T1">The return type of the first parser</typeparam>
	///<typeparam name="T2">The return type of the second parser</typeparam>
	///<typeparam name="T3">The return type of the third parser</typeparam>
	///<typeparam name="T4">The return type of the fourth parser</typeparam>
	///<typeparam name="T5">The return type of the fifth parser</typeparam>
	///<typeparam name="T6">The return type of the sixth parser</typeparam>
	///<typeparam name="T7">The return type of the seventh parser</typeparam>
	///<typeparam name="T8">The return type of the eighth parser</typeparam>
	/// <typeparam name="Tr">The return type of the resulting parser</typeparam>
	public static Parser<TToken, Tr> Map<TToken, T1, T2, T3, T4, T5, T6, T7, T8, Tr>(
		Func<T1, T2, T3, T4, T5, T6, T7, T8, Tr> func,
		Parser<TToken, T1> parser1,
		Parser<TToken, T2> parser2,
		Parser<TToken, T3> parser3,
		Parser<TToken, T4> parser4,
		Parser<TToken, T5> parser5,
		Parser<TToken, T6> parser6,
		Parser<TToken, T7> parser7,
		Parser<TToken, T8> parser8
	)
	{
		ArgumentNullException.ThrowIfNull(func);
		ArgumentNullException.ThrowIfNull(parser1);
		ArgumentNullException.ThrowIfNull(parser2);
		ArgumentNullException.ThrowIfNull(parser3);
		ArgumentNullException.ThrowIfNull(parser4);
		ArgumentNullException.ThrowIfNull(parser5);
		ArgumentNullException.ThrowIfNull(parser6);
		ArgumentNullException.ThrowIfNull(parser7);
		ArgumentNullException.ThrowIfNull(parser8);

		return new Map8Parser<TToken, T1, T2, T3, T4, T5, T6, T7, T8, Tr>(func, parser1, parser2, parser3, parser4, parser5, parser6, parser7, parser8);
	}
}

internal abstract class MapParserBase<TToken, T> : Parser<TToken, T>
{
	internal abstract new MapParserBase<TToken, U> Map<U>(Func<T, U> func);
}

internal sealed class Map1Parser<TToken, T1, R>(
    Func<T1, R> func,
    Parser<TToken, T1> parser1) : MapParserBase<TToken, R>
{
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, out R result)
	{

		var success1 = parser1.TryParse(ref state, ref expected, out var result1);
		if (!success1)
		{
			result = default;
			return false;
		}

		result = func(
			result1
		);
		return true;
	}

	internal override MapParserBase<TToken, U> Map<U>(Func<R, U> func1)
		=> new Map1Parser<TToken, T1, U>(
			(x1) => func1(func(x1)),
			parser1
		);
}

internal sealed class Map2Parser<TToken, T1, T2, R>(
    Func<T1, T2, R> func,
    Parser<TToken, T1> parser1,
    Parser<TToken, T2> parser2)
    : MapParserBase<TToken, R>
{
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, out R result)
	{

		var success1 = parser1.TryParse(ref state, ref expected, out var result1);
		if (!success1)
		{
			result = default;
			return false;
		}

		var success2 = parser2.TryParse(ref state, ref expected, out var result2);
		if (!success2)
		{
			result = default;
			return false;
		}

		result = func(
			result1,
			result2
		);
		return true;
	}

	internal override MapParserBase<TToken, U> Map<U>(Func<R, U> func1)
		=> new Map2Parser<TToken, T1, T2, U>(
			(x1, x2) => func1(func(x1, x2)),
			parser1,
			parser2
		);
}

internal sealed class Map3Parser<TToken, T1, T2, T3, R>(
    Func<T1, T2, T3, R> func,
    Parser<TToken, T1> parser1,
    Parser<TToken, T2> parser2,
    Parser<TToken, T3> parser3)
    : MapParserBase<TToken, R>
{
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, out R result)
	{

		var success1 = parser1.TryParse(ref state, ref expected, out var result1);
		if (!success1)
		{
			result = default;
			return false;
		}

		var success2 = parser2.TryParse(ref state, ref expected, out var result2);
		if (!success2)
		{
			result = default;
			return false;
		}

		var success3 = parser3.TryParse(ref state, ref expected, out var result3);
		if (!success3)
		{
			result = default;
			return false;
		}

		result = func(
			result1,
			result2,
			result3
		);
		return true;
	}

	internal override MapParserBase<TToken, U> Map<U>(Func<R, U> func1)
		=> new Map3Parser<TToken, T1, T2, T3, U>(
			(x1, x2, x3) => func1(func(x1, x2, x3)),
			parser1,
			parser2,
			parser3
		);
}

internal sealed class Map4Parser<TToken, T1, T2, T3, T4, R>(
    Func<T1, T2, T3, T4, R> func,
    Parser<TToken, T1> parser1,
    Parser<TToken, T2> parser2,
    Parser<TToken, T3> parser3,
    Parser<TToken, T4> parser4)
    : MapParserBase<TToken, R>
{
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, out R result)
	{

		var success1 = parser1.TryParse(ref state, ref expected, out var result1);
		if (!success1)
		{
			result = default;
			return false;
		}

		var success2 = parser2.TryParse(ref state, ref expected, out var result2);
		if (!success2)
		{
			result = default;
			return false;
		}

		var success3 = parser3.TryParse(ref state, ref expected, out var result3);
		if (!success3)
		{
			result = default;
			return false;
		}

		var success4 = parser4.TryParse(ref state, ref expected, out var result4);
		if (!success4)
		{
			result = default;
			return false;
		}

		result = func(
			result1,
			result2,
			result3,
			result4
		);
		return true;
	}

	internal override MapParserBase<TToken, U> Map<U>(Func<R, U> func1)
		=> new Map4Parser<TToken, T1, T2, T3, T4, U>(
			(x1, x2, x3, x4) => func1(func(x1, x2, x3, x4)),
			parser1,
			parser2,
			parser3,
			parser4
		);
}

internal sealed class Map5Parser<TToken, T1, T2, T3, T4, T5, R>(
    Func<T1, T2, T3, T4, T5, R> func,
    Parser<TToken, T1> parser1,
    Parser<TToken, T2> parser2,
    Parser<TToken, T3> parser3,
    Parser<TToken, T4> parser4,
    Parser<TToken, T5> parser5)
    : MapParserBase<TToken, R>
{
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, out R result)
	{

		var success1 = parser1.TryParse(ref state, ref expected, out var result1);
		if (!success1)
		{
			result = default;
			return false;
		}

		var success2 = parser2.TryParse(ref state, ref expected, out var result2);
		if (!success2)
		{
			result = default;
			return false;
		}

		var success3 = parser3.TryParse(ref state, ref expected, out var result3);
		if (!success3)
		{
			result = default;
			return false;
		}

		var success4 = parser4.TryParse(ref state, ref expected, out var result4);
		if (!success4)
		{
			result = default;
			return false;
		}

		var success5 = parser5.TryParse(ref state, ref expected, out var result5);
		if (!success5)
		{
			result = default;
			return false;
		}

		result = func(
			result1,
			result2,
			result3,
			result4,
			result5
		);
		return true;
	}

	internal override MapParserBase<TToken, U> Map<U>(Func<R, U> func1)
		=> new Map5Parser<TToken, T1, T2, T3, T4, T5, U>(
			(x1, x2, x3, x4, x5) => func1(func(x1, x2, x3, x4, x5)),
			parser1,
			parser2,
			parser3,
			parser4,
			parser5
		);
}

internal sealed class Map6Parser<TToken, T1, T2, T3, T4, T5, T6, R>(
    Func<T1, T2, T3, T4, T5, T6, R> func,
    Parser<TToken, T1> parser1,
    Parser<TToken, T2> parser2,
    Parser<TToken, T3> parser3,
    Parser<TToken, T4> parser4,
    Parser<TToken, T5> parser5,
    Parser<TToken, T6> parser6)
    : MapParserBase<TToken, R>
{
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, out R result)
	{

		var success1 = parser1.TryParse(ref state, ref expected, out var result1);
		if (!success1)
		{
			result = default;
			return false;
		}

		var success2 = parser2.TryParse(ref state, ref expected, out var result2);
		if (!success2)
		{
			result = default;
			return false;
		}

		var success3 = parser3.TryParse(ref state, ref expected, out var result3);
		if (!success3)
		{
			result = default;
			return false;
		}

		var success4 = parser4.TryParse(ref state, ref expected, out var result4);
		if (!success4)
		{
			result = default;
			return false;
		}

		var success5 = parser5.TryParse(ref state, ref expected, out var result5);
		if (!success5)
		{
			result = default;
			return false;
		}

		var success6 = parser6.TryParse(ref state, ref expected, out var result6);
		if (!success6)
		{
			result = default;
			return false;
		}

		result = func(
			result1,
			result2,
			result3,
			result4,
			result5,
			result6
		);
		return true;
	}

	internal override MapParserBase<TToken, U> Map<U>(Func<R, U> func1)
		=> new Map6Parser<TToken, T1, T2, T3, T4, T5, T6, U>(
			(x1, x2, x3, x4, x5, x6) => func1(func(x1, x2, x3, x4, x5, x6)),
			parser1,
			parser2,
			parser3,
			parser4,
			parser5,
			parser6
		);
}

internal sealed class Map7Parser<TToken, T1, T2, T3, T4, T5, T6, T7, R>(
    Func<T1, T2, T3, T4, T5, T6, T7, R> func,
    Parser<TToken, T1> parser1,
    Parser<TToken, T2> parser2,
    Parser<TToken, T3> parser3,
    Parser<TToken, T4> parser4,
    Parser<TToken, T5> parser5,
    Parser<TToken, T6> parser6,
    Parser<TToken, T7> parser7)
    : MapParserBase<TToken, R>
{
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, out R result)
	{

		var success1 = parser1.TryParse(ref state, ref expected, out var result1);
		if (!success1)
		{
			result = default;
			return false;
		}

		var success2 = parser2.TryParse(ref state, ref expected, out var result2);
		if (!success2)
		{
			result = default;
			return false;
		}

		var success3 = parser3.TryParse(ref state, ref expected, out var result3);
		if (!success3)
		{
			result = default;
			return false;
		}

		var success4 = parser4.TryParse(ref state, ref expected, out var result4);
		if (!success4)
		{
			result = default;
			return false;
		}

		var success5 = parser5.TryParse(ref state, ref expected, out var result5);
		if (!success5)
		{
			result = default;
			return false;
		}

		var success6 = parser6.TryParse(ref state, ref expected, out var result6);
		if (!success6)
		{
			result = default;
			return false;
		}

		var success7 = parser7.TryParse(ref state, ref expected, out var result7);
		if (!success7)
		{
			result = default;
			return false;
		}

		result = func(
			result1,
			result2,
			result3,
			result4,
			result5,
			result6,
			result7
		);
		return true;
	}

	internal override MapParserBase<TToken, U> Map<U>(Func<R, U> func1)
		=> new Map7Parser<TToken, T1, T2, T3, T4, T5, T6, T7, U>(
			(x1, x2, x3, x4, x5, x6, x7) => func1(func(x1, x2, x3, x4, x5, x6, x7)),
			parser1,
			parser2,
			parser3,
			parser4,
			parser5,
			parser6,
			parser7
		);
}

internal sealed class Map8Parser<TToken, T1, T2, T3, T4, T5, T6, T7, T8, R>(
    Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func,
    Parser<TToken, T1> parser1,
    Parser<TToken, T2> parser2,
    Parser<TToken, T3> parser3,
    Parser<TToken, T4> parser4,
    Parser<TToken, T5> parser5,
    Parser<TToken, T6> parser6,
    Parser<TToken, T7> parser7,
    Parser<TToken, T8> parser8)
    : MapParserBase<TToken, R>
{
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, out R result)
	{

		var success1 = parser1.TryParse(ref state, ref expected, out var result1);
		if (!success1)
		{
			result = default;
			return false;
		}

		var success2 = parser2.TryParse(ref state, ref expected, out var result2);
		if (!success2)
		{
			result = default;
			return false;
		}

		var success3 = parser3.TryParse(ref state, ref expected, out var result3);
		if (!success3)
		{
			result = default;
			return false;
		}

		var success4 = parser4.TryParse(ref state, ref expected, out var result4);
		if (!success4)
		{
			result = default;
			return false;
		}

		var success5 = parser5.TryParse(ref state, ref expected, out var result5);
		if (!success5)
		{
			result = default;
			return false;
		}

		var success6 = parser6.TryParse(ref state, ref expected, out var result6);
		if (!success6)
		{
			result = default;
			return false;
		}

		var success7 = parser7.TryParse(ref state, ref expected, out var result7);
		if (!success7)
		{
			result = default;
			return false;
		}

		var success8 = parser8.TryParse(ref state, ref expected, out var result8);
		if (!success8)
		{
			result = default;
			return false;
		}

		result = func(
			result1,
			result2,
			result3,
			result4,
			result5,
			result6,
			result7,
			result8
		);
		return true;
	}

	internal override MapParserBase<TToken, U> Map<U>(Func<R, U> func1)
		=> new Map8Parser<TToken, T1, T2, T3, T4, T5, T6, T7, T8, U>(
			(x1, x2, x3, x4, x5, x6, x7, x8) => func1(func(x1, x2, x3, x4, x5, x6, x7, x8)),
			parser1,
			parser2,
			parser3,
			parser4,
			parser5,
			parser6,
			parser7,
			parser8
		);
}
