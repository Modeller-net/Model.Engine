namespace Modeller;

public static partial class Parser<TToken>
{
    /// <summary>
    /// Creates a parser that parses and returns a single token.
    /// </summary>
    /// <param name="token">The token to parse.</param>
    /// <returns>A parser that parses and returns a single token.</returns>
    public static Parser<TToken, TToken> Token(TToken token)
        => new TokenParser<TToken>(token);

    /// <summary>
    /// Creates a parser that parses and returns a single token satisfying a predicate.
    /// </summary>
    /// <param name="predicate">A predicate function to apply to a token.</param>
    /// <returns>A parser that parses and returns a single token satisfying a predicate.</returns>
    public static Parser<TToken, TToken> Token(Func<TToken, bool> predicate) =>
        new PredicateTokenParser<TToken>(predicate);
}

internal sealed class TokenParser<TToken>(TToken token) : Parser<TToken, TToken>
{
    private Expected<TToken> _expected;

    private Expected<TToken> Expected
    {
        get
        {
            if (_expected.Tokens.IsDefault)
            {
                _expected = new(ImmutableArray.Create(token));
            }

            return _expected;
        }
    }

    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, [MaybeNullWhen(false)] out TToken result)
    {
        if (!state.HasCurrent)
        {
            state.SetError(Maybe.Nothing<TToken>(), true, state.Location, null);
            expected.Add(Expected);
            result = default;
            return false;
        }

        var token1 = state.Current;
        if (!EqualityComparer<TToken>.Default.Equals(token1, token))
        {
            state.SetError(Maybe.Just(token1), false, state.Location, null);
            expected.Add(Expected);
            result = default;
            return false;
        }

        state.Advance();
        result = token1;
        return true;
    }
}

internal sealed class PredicateTokenParser<TToken>(Func<TToken, bool> predicate) : Parser<TToken, TToken>
{
    public override bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, [MaybeNullWhen(false)] out TToken result)
    {
        if (!state.HasCurrent)
        {
            state.SetError(Maybe.Nothing<TToken>(), true, state.Location, null);
            result = default;
            return false;
        }

        var token = state.Current;
        if (!predicate(token))
        {
            state.SetError(Maybe.Just(token), false, state.Location, null);
            result = default;
            return false;
        }

        state.Advance();
        result = token;
        return true;
    }
}
