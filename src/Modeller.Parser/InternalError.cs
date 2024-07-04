// Copyright (c)  Allan Nielsen.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Modeller;

[StructLayout(LayoutKind.Auto)]
internal readonly struct InternalError<TToken>(Maybe<TToken> unexpected, bool eof, int errorLocation, string? message)
{
    public bool Eof { get; } = eof;

    public Maybe<TToken> Unexpected { get; } = unexpected;

    public int ErrorLocation { get; } = errorLocation;

    public string? Message { get; } = message;
}
