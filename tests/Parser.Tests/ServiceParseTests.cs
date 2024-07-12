using FluentAssertions;
using Modeller.Parsers.Models;

namespace Modeller.ParserTests;

public class ServiceParseTests
{
    [Fact]
    public void ServiceFile_CanParse()
    {
        const string fileContent = """
            service DocumentStorageService : description("the document storage service")
            {
                implements_rpcs {
                    FileStorageRPC
                }
            }
            """;
        var file = (ServiceBuilder)EntityParser.ParseService(fileContent);
        file.Name.Name.Value.Should().Be("DocumentStorageService");
        file.Summary.Value.Should().Be("the document storage service");
    }
}