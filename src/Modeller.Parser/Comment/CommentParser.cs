﻿using static Modeller.Parser;
using static Modeller.Parser<char>;

namespace Modeller.Comment;

/// <summary>
/// Contains functions to build parsers which skip over comments.
/// </summary>
public static class CommentParser
{
    /// <summary>
    /// Creates a parser which runs <paramref name="lineCommentStart"/>, then skips the rest of the line.
    /// </summary>
    /// <param name="lineCommentStart">A parser to recognise a lexeme which starts a line comment.</param>
    /// <typeparam name="T">The return type of the <paramref name="lineCommentStart"/> parser.</typeparam>
    /// <returns>A parser which runs <paramref name="lineCommentStart"/>, then skips the rest of the line.</returns>
    public static Parser<char, Unit> SkipLineComment<T>(Parser<char, T> lineCommentStart)
    {
		var eol = Try(EndOfLine).IgnoreResult();
        return lineCommentStart
            .Then(Any.SkipUntil(End.Or(eol)))
            .Labelled("line comment");
    }

    /// <summary>
    /// Creates a parser which runs <paramref name="blockCommentStart"/>,
    /// then skips everything until <paramref name="blockCommentEnd"/>.
    /// </summary>
    /// <param name="blockCommentStart">A parser to recognise a lexeme which starts a multi-line block comment.</param>
    /// <param name="blockCommentEnd">A parser to recognise a lexeme which ends a multi-line block comment.</param>
    /// <typeparam name="T">The return type of the <paramref name="blockCommentStart"/> parser.</typeparam>
    /// <typeparam name="Tu">The return type of the <paramref name="blockCommentEnd"/> parser.</typeparam>
    /// <returns>
    /// A parser which runs <paramref name="blockCommentStart"/>, then skips everything until <paramref name="blockCommentEnd"/>.
    /// </returns>
    public static Parser<char, Unit> SkipBlockComment<T, Tu>(Parser<char, T> blockCommentStart, Parser<char, Tu> blockCommentEnd)=>
		 blockCommentStart
            .Then(Any.SkipUntil(blockCommentEnd))
            .Labelled("block comment");

    /// <summary>
    /// Creates a parser which runs <paramref name="blockCommentStart"/>,
    /// then skips everything until <paramref name="blockCommentEnd"/>, accounting for nested comments.
    /// </summary>
    /// <param name="blockCommentStart">A parser to recognise a lexeme which starts a multi-line block comment.</param>
    /// <param name="blockCommentEnd">A parser to recognise a lexeme which ends a multi-line block comment.</param>
    /// <typeparam name="T">The return type of the <paramref name="blockCommentStart"/> parser.</typeparam>
    /// <typeparam name="Tu">The return type of the <paramref name="blockCommentEnd"/> parser.</typeparam>
    /// <returns>
    /// A parser which runs <paramref name="blockCommentStart"/>,
    /// then skips everything until <paramref name="blockCommentEnd"/>, accounting for nested comments.
    /// </returns>
    public static Parser<char, Unit> SkipNestedBlockComment<T, Tu>(Parser<char, T> blockCommentStart, Parser<char, Tu> blockCommentEnd)
    {
		Parser<char, Unit>? parser = null;

        parser = blockCommentStart.Then(
            Rec(() => parser!).Or(Any.IgnoreResult()).SkipUntil(blockCommentEnd)
        ).Labelled("block comment");

        return parser;
    }
}
