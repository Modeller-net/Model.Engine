using Modeller.Comment;

namespace Modeller.ParserTests;

public class CommentParserTests : ParserTestBase
{
	[Theory]
	[InlineData("//\n")]
	[InlineData("//")]
	[InlineData("// here is a comment ending with an osx style newline\n")]
	[InlineData("// here is a comment ending with a windows style newline\r\n")]
	[InlineData("// here is a comment with a \r carriage return in the middle\r\n")]
	[InlineData("// here is a comment at the end of a file")]
	public void TestSkipLineComment(string comment) => TestCommentParser(
		CommentParser.SkipLineComment(Parser.String("//")).Then(Parser<char>.End),
		comment
	);

	[Theory]
	[InlineData("/**/")]
	[InlineData("/* here is a block comment with \n newlines in */")]
	public void TestSkipBlockComment(string comment) => TestCommentParser(
		CommentParser.SkipBlockComment(Parser.String("/*"), Parser.String("*/")).Then(Parser<char>.End),
		comment
	);

	[Theory]
	[InlineData("/**/")]
	[InlineData("/*/**/*/")]
	[InlineData("/* here is a non-nested block comment with \n newlines in */")]
	[InlineData("/* here is a /* nested */ block comment with \n newlines in */")]
	public void TestSkipNestedBlockComment(string comment) => TestCommentParser(
		CommentParser.SkipNestedBlockComment(Parser.String("/*"), Parser.String("*/")).Then(Parser<char>.End),
		comment
	);

	private static void TestCommentParser(Parser<char, Unit> parser, string comment) => AssertFullParse(parser, comment, Unit.Value);
}
