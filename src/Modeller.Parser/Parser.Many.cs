﻿namespace Modeller;

public static partial class Parser
{
    /// <summary>
    /// Creates a parser which applies the current parser zero or more times, packing the resulting characters into a string.
    /// Equivalent to <c>parser.Many().Select(cs => string.Concat(cs))</c>.
    /// </summary>
    /// <param name="parser">A parser returning a single character.</param>
    /// <typeparam name="TToken">The type of the tokens in the parser's input stream.</typeparam>
    /// <returns>A parser which applies the current parser zero or more times, packing the resulting characters into a string.</returns>
    public static Parser<TToken, string> ManyString<TToken>(this Parser<TToken, char> parser)
    {
		ArgumentNullException.ThrowIfNull(parser);

		return parser.AtLeastOnceString().Or(Parser<TToken>.Return(""));
    }

    /// <summary>
    /// Creates a parser which applies the current parser zero or more times, concatenating the resulting string pieces.
    /// Equivalent to <c>parser.AtLeastOnce().Select(cs => string.Concat(cs))</c>.
    /// </summary>
    /// <param name="parser">A parser returning a single character.</param>
    /// <typeparam name="TToken">The type of the tokens in the parser's input stream.</typeparam>
    /// <returns>A parser which applies the current parser zero or more times, concatenating the resulting string pieces.</returns>
    public static Parser<TToken, string> ManyString<TToken>(this Parser<TToken, string> parser)
    {
		ArgumentNullException.ThrowIfNull(parser);

		return parser.AtLeastOnceString().Or(Parser<TToken>.Return(""));
    }

    /// <summary>
    /// Creates a parser which applies the current parser one or more times, packing the resulting characters into a string.
    /// Equivalent to <c>parser.AtLeastOnce().Select(cs => string.Concat(cs))</c>.
    /// </summary>
    /// <param name="parser">A parser returning a single character.</param>
    /// <typeparam name="TToken">The type of the tokens in the parser's input stream.</typeparam>
    /// <returns>A parser which applies the current parser one or more times, packing the resulting characters into a string.</returns>
    public static Parser<TToken, string> AtLeastOnceString<TToken>(this Parser<TToken, char> parser)
    {
		ArgumentNullException.ThrowIfNull(parser);

		return parser.AtLeastOncePooled()
            .Select(sb => GetStringAndDispose(sb));
    }

    /// <summary>
    /// Creates a parser which applies the current parser one or more times, concatenating the resulting string pieces.
    /// Equivalent to <c>parser.Many().Select(cs => string.Concat(cs))</c>.
    /// </summary>
    /// <param name="parser">A parser returning a single character.</param>
    /// <typeparam name="TToken">The type of the tokens in the parser's input stream.</typeparam>
    /// <returns>A parser which applies the current parser one or more times, concatenating the resulting string pieces.</returns>
    public static Parser<TToken, string> AtLeastOnceString<TToken>(this Parser<TToken, string> parser)
    {
		ArgumentNullException.ThrowIfNull(parser);

		return parser.ChainAtLeastOnce<string, ChunkedStringChainer>(c => new(c.ArrayPoolProvider.GetArrayPool<char>()));
    }

    private struct ChunkedStringChainer(ArrayPool<char> arrayPool) : IChainer<string, string>
    {
        private PooledList<char> _list = new(arrayPool);

        public void Apply(string value)
        {
            _list.AddRange(value.AsSpan());
        }

        public string GetResult() => GetStringAndDispose(_list);

        public void OnError()
        {
            _list.Dispose();
        }
    }

    private static string GetStringAndDispose(PooledList<char> sb)
    {
        var str = sb.AsSpan().ToString();
        sb.Dispose();
        return str;
    }
}

public abstract partial class Parser<TToken, T>
{
    private static Parser<TToken, IEnumerable<T>>? s_returnEmptyEnumerable;

    private static Parser<TToken, IEnumerable<T>> ReturnEmptyEnumerable
    {
        get
        {
            if (s_returnEmptyEnumerable == null)
            {
                s_returnEmptyEnumerable = Parser<TToken>.Return(Enumerable.Empty<T>());
            }

            return s_returnEmptyEnumerable;
        }
    }

    private static Parser<TToken, Unit>? s_returnUnit;

    private static Parser<TToken, Unit> ReturnUnit
    {
        get
        {
            if (s_returnUnit == null)
            {
                s_returnUnit = Parser<TToken>.Return(Unit.Value);
            }

            return s_returnUnit;
        }
    }

    /// <summary>
    /// Creates a parser which applies the current parser zero or more times.
    /// The resulting parser fails if the current parser fails after consuming input.
    /// </summary>
    /// <returns>A parser which applies the current parser zero or more times.</returns>
    public Parser<TToken, IEnumerable<T>> Many()
        => AtLeastOnce()
            .Or(ReturnEmptyEnumerable);

    /// <summary>
    /// Creates a parser that applies the current parser one or more times.
    /// The resulting parser fails if the current parser fails the first time it is applied, or if the current parser fails after consuming input.
    /// </summary>
    /// <returns>A parser that applies the current parser one or more times.</returns>
    public Parser<TToken, IEnumerable<T>> AtLeastOnce()
        => ChainAtLeastOnce<IEnumerable<T>, ListChainer>(c => ListChainer.Create());

    private struct ListChainer : IChainer<T, IEnumerable<T>>
    {
        private readonly List<T> List { get; init; }

        public void Apply(T value)
        {
            List.Add(value);
        }

        public IEnumerable<T> GetResult()
        {
            return List;
        }

        public void OnError()
        {
        }

        public static ListChainer Create()
            => new() { List = new() };
    }

    internal Parser<TToken, PooledList<T>> AtLeastOncePooled()
        => ChainAtLeastOnce<PooledList<T>, PooledListChainer>(c => new(c.ArrayPoolProvider.GetArrayPool<T>()));

    private struct PooledListChainer(ArrayPool<T> arrayPool) : IChainer<T, PooledList<T>>
    {
        private PooledList<T> _list = new(arrayPool);

        public void Apply(T value)
        {
            _list.Add(value);
        }

        public PooledList<T> GetResult()
        {
            return _list;
        }

        public void OnError()
        {
            _list.Dispose();
        }
    }

    /// <summary>
    /// Creates a parser which applies the current parser zero or more times, discarding the results.
    /// This is more efficient than <see cref="Many()"/>, if you don't need the results.
    /// The resulting parser fails if the current parser fails after consuming input.
    /// </summary>
    /// <returns>A parser which applies the current parser zero or more times.</returns>
    public Parser<TToken, Unit> SkipMany()
        => SkipAtLeastOnce()
            .Or(ReturnUnit);

    /// <summary>
    /// Creates a parser that applies the current parser one or more times, discarding the results.
    /// This is more efficient than <see cref="AtLeastOnce()"/>, if you don't need the results.
    /// The resulting parser fails if the current parser fails the first time it is applied, or if the current parser fails after consuming input.
    /// </summary>
    /// <returns>A parser that applies the current parser one or more times, discarding the results.</returns>
    public Parser<TToken, Unit> SkipAtLeastOnce()
        => ChainAtLeastOnce<Unit, NullChainer>(c => default);

    private struct NullChainer : IChainer<T, Unit>
    {
        public void Apply(T value)
        {
        }

        public Unit GetResult() => Unit.Value;

        public void OnError()
        {
        }
    }
}
