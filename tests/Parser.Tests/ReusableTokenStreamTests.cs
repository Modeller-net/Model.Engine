﻿using Modeller.TokenStreams;

namespace Modeller.ParserTests;

public class ReusableTokenStreamTests
{
	[Fact]
	public void TestResume()
	{
		var input = "aaabb";
		using var stream = new ResumableTokenStream<char>(new ReaderTokenStream(new StringReader(input)));

		// consume two 'a's, reject the third one
		var chunk = new char[3].AsSpan();
		stream.Read(chunk);
		stream.Return(chunk[2..]);

		stream.Read(chunk);
		Assert.Equal("abb", chunk.ToString());
	}

	[Fact]
	public void TestReturnMultipleChunks()
	{
		using var stream = new ResumableTokenStream<char>(new ReaderTokenStream(new StringReader("cc")));
		stream.Return("bb");
		stream.Return("aa");

		var chunk = new char[6].AsSpan();
		stream.Read(chunk);
		Assert.Equal("aabbcc", chunk.ToString());
	}

	[Fact]
	public void TestReturnMultipleChunks_GrowBuffer()
	{
		using var stream = new ResumableTokenStream<char>(new ReaderTokenStream(new StringReader("cc")));
		stream.Return("bb");
		stream.Return(new string('a', 16));  // default buffer size is 16

		var chunk = new char[20].AsSpan();
		stream.Read(chunk);
		Assert.Equal(new string('a', 16) + "bbcc", chunk.ToString());
	}
}
