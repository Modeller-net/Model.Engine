using System.IO.Abstractions;
using Domain;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Modeller.NET.Tool.Core;
using Names;
using Spectre.Console;

namespace Modeller.Tests;

public class DefinitionCommandTests
{
    private record MockDefinition : IDefinition
    {
        public Enterprise Create()
        {
            return new Enterprise("TestEnterprise", NameType.FromString("Test Project"), new("TestDescription"));
        }
    }

    private class DefinitionMetaData : IDefinitionMetaData
    {
        public FileVersion Version { get; } = new FileVersion("1.0");
        public string Name { get; } = "TestDefinition";
        public string Description { get; } = "TestDescription";
        public Type EntryPoint { get; } = typeof(MockDefinition);
    }
    
    private class MockDefintion : IDefinitionItem
    {
        public string AbbreviatedFileName { get; } = "TestDefinition";
        public string FilePath { get; } = "TestFolder";
        public IDefinitionMetaData Metadata { get; } = new DefinitionMetaData();
        public Type Type { get; } = typeof(MockDefinition);
        public IDefinition Instance(params object[] args)
        {
            return new MockDefinition();
        }
    }

    [Fact]
    public void Execute_ShouldReturnZero_WhenValidSettingsProvided()
    {
        // Arrange
        var console = Substitute.For<IAnsiConsole>();
        var definitionLoader = Substitute.For<ILoader<IDefinitionItem>>();
        IDefinitionItem? x = new MockDefintion();
        definitionLoader.Load(Arg.Any<string>()).Returns(x);
        var writer = Substitute.For<IDefinitionWriter>();
        var fs = Substitute.For<IFileSystem>();
        fs.File.Exists(Arg.Any<string>()).Returns(true);
        var logger = Substitute.For<ILogger>();
        var settings = new DefinitionSettings
        {
            DefinitionName = "TestDefinition",
            DefinitionFolder = "TestFolder",
            Target = "net8.0",
            OutputFolder = "TestOutputFolder",
            LogFile = "modeller.log"
        };
        IDefinitions command = new Definitions(console, definitionLoader, writer, fs, logger);

        // Act
        var result = command.Create(settings);

        // Assert
        definitionLoader.Received(1).Load(Arg.Any<string>());
        writer.Received().Write(Arg.Any<string>(), Arg.Any<Enterprise>());
        result.Should().BeTrue();
    }

    [Fact]
    public void Execute_ShouldReturnOne_WhenInvalidSettingsProvided()
    {
        // Arrange
        var console = Substitute.For<IAnsiConsole>();
        var definitionLoader = Substitute.For<ILoader<IDefinitionItem>>();
        var writer = Substitute.For<IDefinitionWriter>();
        var fs = Substitute.For<IFileSystem>();
        fs.File.Exists(Arg.Any<string>()).Returns(false);
        var logger = Substitute.For<ILogger>();
        var settings = new DefinitionSettings
        {
            DefinitionName = null,
            DefinitionFolder = null,
            Target = "net8.0",
            OutputFolder = "TestOutputFolder",
            LogFile = "modeller.log"
        };
        IDefinitions command = new Definitions(console, definitionLoader, writer, fs, logger);

        // Act
        var result = command.Create(settings);

        // Assert
        result.Should().BeFalse();
    }
}

