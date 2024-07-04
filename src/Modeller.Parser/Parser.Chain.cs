using Modeller.Configuration;

namespace Modeller;

public partial class Parser<TToken, T>
{
    internal Parser<TToken, Tu> ChainAtLeastOnce<Tu, TChainer>(Func<IConfiguration<TToken>, TChainer> factory)
        where TChainer : struct, IChainer<T, Tu>
        => new ChainAtLeastOnceLParser<TToken, T, Tu, TChainer>(this, factory);
}

internal interface IChainer<in T, out Tu>
{
    void Apply(T value);

    Tu GetResult();

    void OnError();
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:FileMayOnlyContainASingleType",
    Justification = "This class belongs next to the accompanying API method"
)]
internal class ChainAtLeastOnceLParser<TToken, T, Tu, TChainer>(
    Parser<TToken, T> parser,
    Func<IConfiguration<TToken>, TChainer> factory)
    : Parser<TToken, Tu>
    where TChainer : struct, IChainer<T, Tu>
{
    public override sealed bool TryParse(ref ParseState<TToken> state, ref PooledList<Expected<TToken>> expected, [MaybeNullWhen(false)] out Tu result)
    {
        if (!parser.TryParse(ref state, ref expected, out var result1))
        {
            // state.Error set by _parser
            result = default;
            return false;
        }

        var chainer = factory(state.Configuration);
        chainer.Apply(result1);

        var lastStartLoc = state.Location;
        var childExpecteds = new PooledList<Expected<TToken>>(state.Configuration.ArrayPoolProvider.GetArrayPool<Expected<TToken>>());
        while (parser.TryParse(ref state, ref childExpecteds, out var childResult))
        {
            var endLoc = state.Location;
            childExpecteds.Clear();
            if (endLoc <= lastStartLoc)
            {
                childExpecteds.Dispose();
                chainer.OnError();
                throw new InvalidOperationException("Many() used with a parser which consumed no input");
            }

            chainer.Apply(childResult);

            lastStartLoc = endLoc;
        }

        var lastParserConsumedInput = state.Location > lastStartLoc;
        if (lastParserConsumedInput)
        {
            expected.AddRange(childExpecteds.AsSpan());
        }

        childExpecteds.Dispose();

        if (lastParserConsumedInput)
        {
            // the most recent parser failed after consuming input.
            // state.Error was set by _parser
            chainer.OnError();
            result = default;
            return false;
        }

        result = chainer.GetResult();
        return true;
    }
}
