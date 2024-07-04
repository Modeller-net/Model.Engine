﻿namespace Modeller.Configuration;

/// <summary>
/// Methods for creating and updating <see cref="IConfiguration{TToken}"/>s.
/// </summary>
[SuppressMessage(
    "naming",
    "CA1724:The type name conflicts with the namespace name 'System.Configuration'",
    Justification = "Would be a breaking change"
)]
public static class Configuration
{
    /// <summary>
    /// Returns the default configuration for the token type <typeparamref name="TToken"/>.
    /// </summary>
    /// <typeparam name="TToken">The token type.</typeparam>
    /// <returns>The default configuration for the token type <typeparamref name="TToken"/>.</returns>
    public static IConfiguration<TToken> Default<TToken>()
        => typeof(TToken) == typeof(char)
            ? (IConfiguration<TToken>)CharDefaultConfiguration.Instance
            : DefaultConfiguration<TToken>.Instance;

    /// <summary>
    /// Override the <see cref="IConfiguration{TToken}.SourcePosCalculator"/>.
    /// </summary>
    /// <typeparam name="TToken">The token type.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="posCalculator">The new <see cref="IConfiguration{TToken}.SourcePosCalculator"/>.</param>
    /// <returns>
    /// A copy of <paramref name="configuration"/> with its <see cref="IConfiguration{TToken}.SourcePosCalculator"/> overridden.
    /// </returns>
    public static IConfiguration<TToken> WithPosCalculator<TToken>(
        this IConfiguration<TToken> configuration,
        Func<TToken, SourcePosDelta> posCalculator)
    {
		ArgumentNullException.ThrowIfNull(configuration);
		ArgumentNullException.ThrowIfNull(posCalculator);

		return new OverrideConfiguration<TToken>(configuration, posCalculator: posCalculator);
    }

    /// <summary>
    /// Override the <see cref="IConfiguration{TToken}.ArrayPoolProvider"/>.
    /// </summary>
    /// <typeparam name="TToken">The token type.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="arrayPoolProvider">The new <see cref="IConfiguration{TToken}.ArrayPoolProvider"/>.</param>
    /// <returns>
    /// A copy of <paramref name="configuration"/> with its <see cref="IConfiguration{TToken}.ArrayPoolProvider"/> overridden.
    /// </returns>
    public static IConfiguration<TToken> WithArrayPoolProvider<TToken>(
        this IConfiguration<TToken> configuration,
        IArrayPoolProvider arrayPoolProvider)
    {
		ArgumentNullException.ThrowIfNull(configuration);
		ArgumentNullException.ThrowIfNull(arrayPoolProvider);

		return new OverrideConfiguration<TToken>(configuration, arrayPoolProvider: arrayPoolProvider);
    }
}
