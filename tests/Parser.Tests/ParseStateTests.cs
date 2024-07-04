﻿using Modeller.Configuration;
using Modeller.TokenStreams;

namespace Modeller.ParserTests;

public class ParseStateTests
{
    [Fact]
    public void TestEmptyInput()
    {
        var input = "";
        using var state = new ParseState<char>(CharDefaultConfiguration.Instance, ToStream(input));

        Assert.Equal(SourcePosDelta.Zero, state.ComputeSourcePosDelta());
        Assert.False(state.HasCurrent);
    }

    [Fact]
    public void TestAdvance()
    {
        var input = "foo";
        var state = new ParseState<char>(CharDefaultConfiguration.Instance, ToStream(input));

        Consume('f', ref state);
        Consume('o', ref state);
        Consume('o', ref state);

        Assert.False(state.HasCurrent);
    }

    [Fact]
    public void TestDiscardChunk()
    {
        var input = 'f' + new string('o', ChunkSize);  // Length == ChunkSize + 1
        var state = new ParseState<char>(CharDefaultConfiguration.Instance, ToStream(input));

        Consume('f', ref state);
        Consume(new string('o', ChunkSize), ref state);
        Assert.False(state.HasCurrent);
        Assert.Equal(new(0, input.Length), state.ComputeSourcePosDelta());
    }

    [Fact]
    public void TestSaveWholeChunkAligned() =>
	    // grows buffer on final iteration of loop
	    //
	    // |----|----|
	    // foooo
	    // ^----
	    AlignedChunkTest(ChunkSize);

    [Fact]
    public void TestSaveWholeChunkUnaligned() =>
	    // grows buffer on final iteration of loop
	    //
	    // |----|----|
	    // faoooo
	    //  ^----
	    UnalignedChunkTest(ChunkSize);

    [Fact]
    public void TestSaveMoreThanWholeChunkAligned() =>
	    // grows buffer on penultimate iteration of loop
	    //
	    // |----|----|
	    // fooooo
	    // ^-----
	    AlignedChunkTest(ChunkSize + 1);

    [Fact]
    public void TestSaveMoreThanWholeChunkUnaligned() =>
	    // grows buffer on penultimate iteration of loop
	    //
	    // |----|----|
	    // faoooo
	    //  ^----
	    UnalignedChunkTest(ChunkSize + 1);

    [Fact]
    public void TestSaveLessThanWholeChunkAligned() =>
	    // does not grow buffer
	    //
	    // |----|----|
	    // fooooo
	    // ^-----
	    AlignedChunkTest(ChunkSize - 1);

    [Fact]
    public void TestSaveLessThanWholeChunkUnaligned() =>
	    // does not grow buffer
	    //
	    // |----|----|
	    // fooooo
	    // ^-----
	    UnalignedChunkTest(ChunkSize - 1);

    [Fact]
    public void TestComputeSourcePos_Default()
    {
        {
            var input = "a\n\nb";
            using var state = new ParseState<char>(DefaultConfiguration<char>.Instance, input.AsSpan());

            state.Advance(input.Length);

            Assert.Equal(new(0, 4), state.ComputeSourcePosDelta());
        }
    }

    [Fact]
    public void TestComputeSourcePos_CharDefault()
    {
        var input = "a\n\nb" // a partial chunk containing multiple newlines
            + "\n" + new string('a', Vector<short>.Count - 2) + "\n" // multiple whole chunks with multiple newlines
            + "\n" + new string('a', Vector<short>.Count - 2) + "\n" // ...
            + "\t" + new string('a', (Vector<short>.Count * 2) - 2) + "\t" // multiple whole chunks with tabs and no newlines
            + "aa";  // a partial chunk with no newlines
        {
            using var state = new ParseState<char>(CharDefaultConfiguration.Instance, input.AsSpan());

            state.Advance(input.Length);

            Assert.Equal(new(6, (Vector<short>.Count * 2) + 8), state.ComputeSourcePosDelta());
        }

        {
            using var state = new ParseState<char>(CharDefaultConfiguration.Instance, input.AsSpan());

            state.Advance(1);
            state.ComputeSourcePosDelta();
            state.Advance(input.Length - 1);

            Assert.Equal(new(6, (Vector<short>.Count * 2) + 8), state.ComputeSourcePosDelta());
        }
    }

    private static void AlignedChunkTest(int inputLength)
    {
        var input = 'f' + new string('o', inputLength - 1);
        var state = new ParseState<char>(CharDefaultConfiguration.Instance, ToStream(input));

        var bookmark = state.Bookmark();

        Consume('f', ref state);
        Consume(new string('o', inputLength - 1), ref state);
        Assert.False(state.HasCurrent);
        Assert.Equal(new(0, inputLength), state.ComputeSourcePosDelta());

        state.Rewind(bookmark);
        Assert.Equal(SourcePosDelta.Zero, state.ComputeSourcePosDelta());
        Consume('f', ref state);
    }

    private static void UnalignedChunkTest(int inputLength)
    {
        var input = "fa" + new string('o', inputLength - 2);
        var state = new ParseState<char>(CharDefaultConfiguration.Instance, ToStream(input));

        Consume('f', ref state);

        var bookmark = state.Bookmark();
        Consume('a' + new string('o', inputLength - 2), ref state);
        Assert.False(state.HasCurrent);
        Assert.Equal(new(0, inputLength), state.ComputeSourcePosDelta());

        state.Rewind(bookmark);
        Assert.Equal(SourcePosDelta.OneCol, state.ComputeSourcePosDelta());
        Consume('a', ref state);
    }

    private static void Consume(char expected, ref ParseState<char> state)
    {
        var oldCols = state.ComputeSourcePosDelta().Cols;
        Assert.True(state.HasCurrent);
        Assert.Equal(expected, state.Current);
        state.Advance();
        Assert.Equal(oldCols + 1, state.ComputeSourcePosDelta().Cols);
    }

    private static void Consume(string expected, ref ParseState<char> state)
    {
        var oldCols = state.ComputeSourcePosDelta().Cols;
        AssertEqual(expected.AsSpan(), state.LookAhead(expected.Length));
        state.Advance(expected.Length);
        Assert.Equal(oldCols + expected.Length, state.ComputeSourcePosDelta().Cols);
    }

    private static void AssertEqual(ReadOnlySpan<char> expected, ReadOnlySpan<char> actual)
    {
        Assert.Equal(expected.Length, actual.Length);
        for (var i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], actual[i]);
        }
    }

#pragma warning disable CA1859
    private static ITokenStream<char> ToStream(string input)
#pragma warning restore CA1859
	    => new ReaderTokenStream(new StringReader(input));

    private static int ChunkSize => ToStream("").ChunkSizeHint;
}
