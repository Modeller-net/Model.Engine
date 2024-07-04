// Copyright (c)  Allan Nielsen.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Modeller.Configuration;

/// <summary>
/// A default configuration for any token type.
/// </summary>
/// <typeparam name="TToken">The token type.</typeparam>
public class DefaultConfiguration<TToken> : IConfiguration<TToken>
{
    /// <summary>
    /// The shared global instance of <see cref="DefaultConfiguration{TToken}"/>.
    /// </summary>
    [SuppressMessage(
        "design",
        "CA1000:Do not declare static members on generic types",
        Justification = "Typically won't be used at a large number of different types"
    )]
    public static IConfiguration<TToken> Instance { get; } = new DefaultConfiguration<TToken>();

    /// <summary>
    /// Always increments the column by 1.
    /// </summary>
    public virtual Func<TToken, SourcePosDelta> SourcePosCalculator { get; }
        = _ => SourcePosDelta.OneCol;

    /// <summary>
    /// Always returns <see cref="DefaultArrayPoolProvider.Instance"/>.
    /// </summary>
    public virtual IArrayPoolProvider ArrayPoolProvider { get; } = DefaultArrayPoolProvider.Instance;
}
