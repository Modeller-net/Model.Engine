using FluentAssertions;
using Modeller.Parsers.Models;

namespace Modeller.ParserTests;

public class ProjectParseTests
{
    [Fact]
    public void ProjectParser_CanParse()
    {
        const string fileContent = """
            ###############################
            # New Branch project
            ###############################

            project dev10.NewBranch : description("New Branch project")
            {
                Company: "JJ Waste"
            }
            """;
        var project = (ProjectBuilder)EntityParser.ParseProject(fileContent);
        project.Name.Value.Value.Should().Be("NewBranch");
        project.Name.Version.HasValue.Should().BeTrue();
        project.Name.Version.Value.Should().Be("dev10");
        project.Summary.Value.Should().Be("New Branch project");
        project.Attributes.Should().HaveCount(1);
        project.Attributes["Company"].Should().Be("JJ Waste");
    } 
}