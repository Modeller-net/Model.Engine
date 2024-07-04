namespace Modeller.Configuration;

internal class OverrideConfiguration<TToken>(
    IConfiguration<TToken> next,
    Func<TToken, SourcePosDelta>? posCalculator = null,
    IArrayPoolProvider? arrayPoolProvider = null)
    : IConfiguration<TToken>
{
	public Func<TToken, SourcePosDelta> SourcePosCalculator { get; } = posCalculator ?? next.SourcePosCalculator;

    public IArrayPoolProvider ArrayPoolProvider { get; } = arrayPoolProvider ?? next.ArrayPoolProvider;
}
