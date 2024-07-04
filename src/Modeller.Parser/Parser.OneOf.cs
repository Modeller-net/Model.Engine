﻿namespace Modeller;

public static partial class Parser
{
    /// <summary>
    /// Creates a parser which parses and returns one of the specified characters.
    /// </summary>
    /// <param name="chars">A sequence of characters to choose between.</param>
    /// <returns>A parser which parses and returns one of the specified characters.</returns>
    public static Parser<char, char> OneOf(params char[] chars)
    {
		ArgumentNullException.ThrowIfNull(chars);

		return OneOf(chars.AsEnumerable());
    }

    /// <summary>
    /// Creates a parser which parses and returns one of the specified characters.
    /// </summary>
    /// <param name="chars">A sequence of characters to choose between.</param>
    /// <returns>A parser which parses and returns one of the specified characters.</returns>
    public static Parser<char, char> OneOf(IEnumerable<char> chars)
    {
        ArgumentNullException.ThrowIfNull(chars);

        var cs = chars.ToArray();
        return Parser<char>
            .Token(c => Array.IndexOf(cs, c) != -1)
            .WithExpected([..cs.Select(c => new Expected<char>(ImmutableArray.Create(c)))]);
    }

    /// <summary>
    /// Creates a parser which parses and returns one of the specified characters, in a case insensitive manner.
    /// The parser returns the actual character parsed.
    /// </summary>
    /// <param name="chars">A sequence of characters to choose between.</param>
    /// <returns>A parser which parses and returns one of the specified characters, in a case insensitive manner.</returns>
    public static Parser<char, char> CIOneOf(params char[] chars)
    {
		ArgumentNullException.ThrowIfNull(chars);

		return CIOneOf(chars.AsEnumerable());
    }

    /// <summary>
    /// Creates a parser which parses and returns one of the specified characters, in a case insensitive manner.
    /// The parser returns the actual character parsed.
    /// </summary>
    /// <param name="chars">A sequence of characters to choose between.</param>
    /// <returns>A parser which parses and returns one of the specified characters, in a case insensitive manner.</returns>
    public static Parser<char, char> CIOneOf(IEnumerable<char> chars)
    {
		ArgumentNullException.ThrowIfNull(chars);

		var cs = chars.Select(char.ToLowerInvariant).ToArray();
        var builder = ImmutableArray.CreateBuilder<Expected<char>>(cs.Length * 2);
        foreach (var c in cs)
        {
            builder.Add(new(ImmutableArray.Create(char.ToLowerInvariant(c))));
            builder.Add(new(ImmutableArray.Create(char.ToUpperInvariant(c))));
        }

        return Parser<char>
            .Token(c => Array.IndexOf(cs, char.ToLowerInvariant(c)) != -1)
            .WithExpected(builder.MoveToImmutable());
    }

    /// <summary>
    /// Creates a parser which applies one of the specified parsers.
    /// The resulting parser fails if all of the input parsers fail without consuming input, or if one of them fails after consuming input.
    /// </summary>
    /// <typeparam name="TToken">The type of tokens in the parsers' input stream.</typeparam>
    /// <typeparam name="T">The return type of the parsers.</typeparam>
    /// <param name="parsers">A sequence of parsers to choose between.</param>
    /// <returns>A parser which applies one of the specified parsers.</returns>
    public static Parser<TToken, T> OneOf<TToken, T>(params Parser<TToken, T>[] parsers)
    {
        ArgumentNullException.ThrowIfNull(parsers);

        return OneOf(parsers.AsEnumerable());
    }

    /// <summary>
    /// Creates a parser which applies one of the specified parsers.
    /// The resulting parser fails if all of the input parsers fail without consuming input, or if one of them fails after consuming input.
    /// The input enumerable is enumerated and copied to a list.
    /// </summary>
    /// <typeparam name="TToken">The type of tokens in the parsers' input stream.</typeparam>
    /// <typeparam name="T">The return type of the parsers.</typeparam>
    /// <param name="parsers">A sequence of parsers to choose between.</param>
    /// <returns>A parser which applies one of the specified parsers.</returns>
    public static Parser<TToken, T> OneOf<TToken, T>(IEnumerable<Parser<TToken, T>> parsers)
    {
        ArgumentNullException.ThrowIfNull(parsers);

        return OneOfParser<TToken, T>.Create(parsers);
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:FileMayOnlyContainASingleType",
    Justification = "This class belongs next to the accompanying API method"
)]
internal sealed class OneOfParser<TToken, T> : Parser<TToken, T>
{
    private readonly Parser<TToken, T>[] _parsers;

    private OneOfParser(Parser<TToken, T>[] parsers)
    {
        _parsers = parsers;
    }

    // see comment about expected in ParseState.Error.cs
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, [MaybeNullWhen(false)] out T result)
    {
        var firstTime = true;
        var err = new InternalError<TToken>(
            Maybe.Nothing<TToken>(),
            false,
            state.Location,
            "OneOf had no arguments"
        );

        var childExpected = new PooledList<Expected<TToken>>(state.Configuration.ArrayPoolProvider.GetArrayPool<Expected<TToken>>());  // the expected for all loop iterations
        var grandchildExpected = new PooledList<Expected<TToken>>(state.Configuration.ArrayPoolProvider.GetArrayPool<Expected<TToken>>());  // the expected for the current loop iteration
        foreach (var p in _parsers)
        {
            var thisStartLoc = state.Location;

            if (p.TryParse(ref state, ref grandchildExpected, out result))
            {
                // throw out all expected
                grandchildExpected.Dispose();
                childExpected.Dispose();
                return true;
            }

            // we'll usually return the error from the first parser that didn't backtrack,
            // even if other parsers had a longer match.
            // There is some room for improvement here.
            if (state.Location > thisStartLoc)
            {
                // throw out all expected except this one
                expected.AddRange(grandchildExpected.AsSpan());
                childExpected.Dispose();
                grandchildExpected.Dispose();
                result = default;
                return false;
            }

            childExpected.AddRange(grandchildExpected.AsSpan());
            grandchildExpected.Clear();

            // choose the longest match, preferring the left-most error in a tie,
            // except the first time (avoid returning "OneOf had no arguments").
            if (firstTime || state.ErrorLocation > err.ErrorLocation)
            {
                err = state.GetError();
            }

            firstTime = false;
        }

        state.SetError(err);
        expected.AddRange(childExpected.AsSpan());
        childExpected.Dispose();
        grandchildExpected.Dispose();
        result = default;
        return false;
    }

    internal static OneOfParser<TToken, T> Create(IEnumerable<Parser<TToken, T>> parsers)
    {
        // if we know the length of the collection,
        // we know we're going to need at least that much room in the list
        var list = parsers is ICollection<Parser<TToken, T>> coll
            ? new(coll.Count)
            : new List<Parser<TToken, T>>();

        foreach (var p in parsers)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(parsers));
            }

            if (p is OneOfParser<TToken, T> o)
            {
                list.AddRange(o._parsers);
            }
            else
            {
                list.Add(p);
            }
        }

        return new(list.ToArray());
    }
}
