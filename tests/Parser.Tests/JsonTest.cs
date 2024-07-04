using Modeller.Parsers;

namespace Modeller.ParserTests;

public class JsonTest
{
	[Fact]
	public void TestJsonObject()
	{
		var input = "[ { \"foo\" : \"bar\" } , [ \"baz\" ] ]";

		var result = JsonParser.Parse(input);

		Assert.Equal("[{\"foo\":\"bar\"},[\"baz\"]]", result.Value.ToString());
	}
}
