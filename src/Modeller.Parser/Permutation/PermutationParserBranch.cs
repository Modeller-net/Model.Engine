namespace Modeller.Permutation;

internal abstract class PermutationParserBranch<TToken, T>
{
	public abstract PermutationParserBranch<TToken, Tr> Add<Tu, Tr>(Parser<TToken, Tu> parser, Func<T, Tu, Tr> resultSelector);

	public abstract PermutationParserBranch<TToken, Tr> AddOptional<Tu, Tr>(Parser<TToken, Tu> parser, Func<Tu> defaultValueFactory, Func<T, Tu, Tr> resultSelector);

	public abstract Parser<TToken, T> Build();
}

#pragma warning disable SA1402  // "File may only contain a single type"
internal sealed class PermutationParserBranchImpl<TToken, Tu, T, Tr>(
    Parser<TToken, Tu> parser,
    PermutationParser<TToken, T> perm,
    Func<T, Tu, Tr> func)
    : PermutationParserBranch<TToken, Tr>
#pragma warning restore SA1402 // "File may only contain a single type"
{
    public override PermutationParserBranch<TToken, Tw> Add<Tv, Tw>(Parser<TToken, Tv> parser, Func<Tr, Tv, Tw> resultSelector)
		=> Add(p => p.Add(parser), resultSelector);

	public override PermutationParserBranch<TToken, Tw> AddOptional<Tv, Tw>(Parser<TToken, Tv> parser, Func<Tv> defaultValueFactory, Func<Tr, Tv, Tw> resultSelector)
		=> Add(p => p.AddOptional(parser, defaultValueFactory), resultSelector);

	private PermutationParserBranch<TToken, Tw> Add<Tv, Tw>(Func<PermutationParser<TToken, T>, PermutationParser<TToken, (T, Tv)>> addPerm, Func<Tr, Tv, Tw> resultSelector)
	{
		var thisFunc = func;
		return new PermutationParserBranchImpl<TToken, Tu, (T, Tv), Tw>(
			parser,
			addPerm(perm),
			(tv, u) => resultSelector(thisFunc(tv.Item1, u), tv.Item2)
		);
	}

	public override Parser<TToken, Tr> Build()
	{
		var thisFunc = func;
		return Parser.Map((x, y) => thisFunc(y, x), parser, perm.Build());
	}
}
