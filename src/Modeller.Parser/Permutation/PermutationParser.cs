namespace Modeller.Permutation;

/// <summary>
/// Contains tools for running sequences of parsers in an order-insensitive manner.
/// </summary>
public static class PermutationParser
{
    /// <summary>
    /// Creates an empty instance of <see cref="PermutationParser{TToken, T}"/>.
    /// </summary>
    /// <typeparam name="TToken">The type of tokens to be consumed by the permutation parser.</typeparam>
    /// <returns>An empty instance of <see cref="PermutationParser{TToken, T}"/>.</returns>
    public static PermutationParser<TToken, Unit> Create<TToken>()
        => new(() => Unit.Value, ImmutableList.Create<PermutationParserBranch<TToken, Unit>>());
}

/// <summary>
/// A permutation parser represents a collection of parsers which can be run in an order-insensitive manner.
///
/// Declaration modifiers in C# are an example of an order-insensitive grammar.
/// Modifiers can appear in any order: <c>protected internal static readonly int x;</c>
/// means the same as <c>internal readonly protected static int x;</c>.
///
/// Usage of this class involves calling <see cref="Add{U}(Parser{TToken, U})"/>
/// or <see cref="AddOptional{U}(Parser{TToken, U}, U)"/> to add parsers to the permutation parser,
/// and then calling <see cref="Build"/> to create a parser which runs them in an order-insensitive manner
/// and returns the results in a nested tuple.
///
/// Note that the parsers that are added to the permutation parser must always consume input before succeeding.
/// If a parser succeeds on empty input the permutation parser will not work correctly.
/// If you want to run a parser optionally, use <see cref="AddOptional{U}(Parser{TToken, U}, U)"/>.
///
/// This class is immutable.
/// </summary>
/// <typeparam name="TToken">The type of the tokens in the parser's input stream.</typeparam>
/// <typeparam name="T">The type of the value returned by the parser.</typeparam>
#pragma warning disable SA1402  // "File may only contain a single type"
public sealed class PermutationParser<TToken, T>
#pragma warning restore SA1402  // "File may only contain a single type"
{
    private readonly Func<T>? _exit;
    private readonly ImmutableList<PermutationParserBranch<TToken, T>> _forest;

    internal PermutationParser(Func<T>? exit, ImmutableList<PermutationParserBranch<TToken, T>> forest)
    {
        _exit = exit;
        _forest = forest;
    }

    /// <summary>
    /// Creates a <see cref="Parser{TToken, T}"/> which runs the current
    /// collection of parsers in an order-insensitive manner.
    /// </summary>
    /// <returns>
    /// A <see cref="Parser{TToken, T}"/> which runs the current collection of parsers in an order-insensitive manner.
    /// </returns>
    public Parser<TToken, T> Build()
    {
        var forest = Parser.OneOf(_forest.Select(t => t.Build()));
        return _exit != null ? forest.Or(Parser<TToken>.Return(_exit).Select(f => f())) : forest;
    }

    /// <summary>
    /// Adds a parser to the collection.
    /// </summary>
    /// <param name="parser">The parser to add to the collection.</param>
    /// <typeparam name="Tu">The return type of the parser to add to the collection.</typeparam>
    /// <returns>
    /// A new permutation parser representing the current collection of parsers with <paramref name="parser"/> added.
    /// </returns>
    public PermutationParser<TToken, (T, Tu)> Add<Tu>(Parser<TToken, Tu> parser)
        => Add(parser, ValueTuple.Create);

    /// <summary>
    /// Adds a parser to the collection.
    /// </summary>
    /// <param name="parser">The parser to add to the collection.</param>
    /// <param name="resultSelector">
    /// A transformation function to apply to the result of the current permutation parser and the result of <paramref name="parser"/>.
    /// </param>
    /// <typeparam name="Tu">The return type of the parser to add to the collection.</typeparam>
    /// <typeparam name="Tr">The return type of the resulting permutation parser.</typeparam>
    /// <returns>
    /// A new permutation parser representing the current collection of parsers with <paramref name="parser"/> added.
    /// </returns>
    public PermutationParser<TToken, Tr> Add<Tu, Tr>(Parser<TToken, Tu> parser, Func<T, Tu, Tr> resultSelector)
    {
		ArgumentNullException.ThrowIfNull(parser);
		ArgumentNullException.ThrowIfNull(resultSelector);

		return new(
            null,
            ConvertForestAndAddParser(b => b.Add(parser, resultSelector), parser, resultSelector)
        );
    }

    /// <summary>
    /// Adds an optional parser to the collection.
    ///
    /// The resulting permutation parser will successfully parse a phrase even if <paramref name="parser"/> never succeeds.
    /// In that case, <see cref="Maybe.Nothing{T}"/> will be returned.
    /// </summary>
    /// <param name="parser">The parser to add to the collection.</param>
    /// <typeparam name="Tu">The return type of the parser to add to the collection.</typeparam>
    /// <returns>
    /// A new permutation parser representing the current collection of parsers with <paramref name="parser"/> added optionally.
    /// </returns>
    public PermutationParser<TToken, (T, Maybe<Tu>)> AddOptional<Tu>(Parser<TToken, Tu> parser)
    {
		ArgumentNullException.ThrowIfNull(parser);

		return AddOptional(parser.Select(Maybe.Just), Maybe.Nothing<Tu>());
    }

    /// <summary>
    /// Adds an optional parser to the collection.
    ///
    /// The resulting permutation parser will successfully parse a phrase even if <paramref name="parser"/> never succeeds.
    /// In that case, <paramref name="defaultValue"/> will be returned.
    /// </summary>
    /// <param name="parser">The parser to add to the collection.</param>
    /// <param name="defaultValue">A default value to return if <paramref name="parser"/> fails.</param>
    /// <typeparam name="Tu">The return type of the parser to add to the collection.</typeparam>
    /// <returns>
    /// A new permutation parser representing the current collection of parsers with <paramref name="parser"/> added optionally.
    /// </returns>
    public PermutationParser<TToken, (T, Tu)> AddOptional<Tu>(Parser<TToken, Tu> parser, Tu defaultValue)
        => AddOptional(parser, () => defaultValue);

    /// <summary>
    /// Adds an optional parser to the collection.
    ///
    /// The resulting permutation parser will successfully parse a phrase even if <paramref name="parser"/> never succeeds.
    /// In that case, <paramref name="defaultValueFactory"/> will be called to get a value to return.
    /// </summary>
    /// <param name="parser">The parser to add to the collection.</param>
    /// <param name="defaultValueFactory">A factory for a default value to return if <paramref name="parser"/> fails.</param>
    /// <typeparam name="Tu">The return type of the parser to add to the collection.</typeparam>
    /// <returns>
    /// A new permutation parser representing the current collection of parsers with <paramref name="parser"/> added optionally.
    /// </returns>
    public PermutationParser<TToken, (T, Tu)> AddOptional<Tu>(Parser<TToken, Tu> parser, Func<Tu> defaultValueFactory)
        => AddOptional(parser, defaultValueFactory, ValueTuple.Create);

    /// <summary>
    /// Adds an optional parser to the collection.
    ///
    /// The resulting permutation parser will successfully parse a phrase even if <paramref name="parser"/> never succeeds.
    /// In that case, <see cref="Maybe.Nothing{T}"/> will be returned.
    /// </summary>
    /// <param name="parser">The parser to add to the collection.</param>
    /// <param name="resultSelector">
    /// A transformation function to apply to the result of the current permutation parser and the result of <paramref name="parser"/>.
    /// </param>
    /// <typeparam name="Tu">The return type of the parser to add to the collection.</typeparam>
    /// <typeparam name="Tr">The return type of the resulting permutation parser.</typeparam>
    /// <returns>
    /// A new permutation parser representing the current collection of parsers with <paramref name="parser"/> added optionally.
    /// </returns>
    public PermutationParser<TToken, Tr> AddOptional<Tu, Tr>(Parser<TToken, Tu> parser, Func<T, Maybe<Tu>, Tr> resultSelector)
    {
		ArgumentNullException.ThrowIfNull(parser);

		return AddOptional(parser.Select(Maybe.Just), () => Maybe.Nothing<Tu>(), resultSelector);
    }

    /// <summary>
    /// Adds an optional parser to the collection.
    ///
    /// The resulting permutation parser will successfully parse a phrase even if <paramref name="parser"/> never succeeds.
    /// In that case, <paramref name="defaultValue"/> will be returned.
    /// </summary>
    /// <param name="parser">The parser to add to the collection.</param>
    /// <param name="defaultValue">A default value to return if <paramref name="parser"/> fails.</param>
    /// <param name="resultSelector">
    /// A transformation function to apply to the result of the current permutation parser and the result of <paramref name="parser"/>.
    /// </param>
    /// <typeparam name="Tu">The return type of the parser to add to the collection.</typeparam>
    /// <typeparam name="Tr">The return type of the resulting permutation parser.</typeparam>
    /// <returns>
    /// A new permutation parser representing the current collection of parsers with <paramref name="parser"/> added optionally.
    /// </returns>
    public PermutationParser<TToken, Tr> AddOptional<Tu, Tr>(Parser<TToken, Tu> parser, Tu defaultValue, Func<T, Tu, Tr> resultSelector)
        => AddOptional(parser, () => defaultValue, resultSelector);

    /// <summary>
    /// Adds an optional parser to the collection.
    ///
    /// The resulting permutation parser will successfully parse a phrase even if <paramref name="parser"/> never succeeds.
    /// In that case, <paramref name="defaultValueFactory"/> will be called to get a value to return.
    /// </summary>
    /// <param name="parser">The parser to add to the collection.</param>
    /// <param name="defaultValueFactory">A factory for a default value to return if <paramref name="parser"/> fails.</param>
    /// <param name="resultSelector">
    /// A transformation function to apply to the result of the current permutation parser and the result of <paramref name="parser"/>.
    /// </param>
    /// <typeparam name="Tu">The return type of the parser to add to the collection.</typeparam>
    /// <typeparam name="Tr">The return type of the resulting permutation parser.</typeparam>
    /// <returns>
    /// A new permutation parser representing the current collection of parsers with <paramref name="parser"/> added optionally.
    /// </returns>
    public PermutationParser<TToken, Tr> AddOptional<Tu, Tr>(Parser<TToken, Tu> parser, Func<Tu> defaultValueFactory, Func<T, Tu, Tr> resultSelector)
    {
		ArgumentNullException.ThrowIfNull(parser);
		ArgumentNullException.ThrowIfNull(defaultValueFactory);
		ArgumentNullException.ThrowIfNull(resultSelector);

		var thisExit = _exit;
        return new(
            thisExit == null ? null : () => resultSelector(thisExit!(), defaultValueFactory()),
            ConvertForestAndAddParser(b => b.AddOptional(parser, defaultValueFactory, resultSelector), parser, resultSelector)
        );
    }

    private ImmutableList<PermutationParserBranch<TToken, Tr>> ConvertForestAndAddParser<Tu, Tr>(
        Func<PermutationParserBranch<TToken, T>, PermutationParserBranch<TToken, Tr>> func,
        Parser<TToken, Tu> parser,
        Func<T, Tu, Tr> resultSelector
    ) => _forest
        .ConvertAll(func)
        .Add(new PermutationParserBranchImpl<TToken, Tu, T, Tr>(parser, this, resultSelector));
}
