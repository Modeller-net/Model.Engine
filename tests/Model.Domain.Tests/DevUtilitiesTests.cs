using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using NSubstitute;

namespace Model.Domain.Tests;

public class DevUtilitiesTests
{
    [Fact]
    public void GetSolutionDirectory_ShouldReturnCorrectPath_WhenDebuggerIsAttached()
    {
        // Arrange
        var folder = "TestFolder";
        var expectedPath = Path.Combine(@"C:\SolutionDirectory", folder);
        var mockDebuggerWrapper = Substitute.For<IDebuggerWrapper>();
        mockDebuggerWrapper.IsAttached.Returns(true);
        mockDebuggerWrapper.ExecutingAssemblyLocation.Returns(@"C:\SolutionDirectory\bin\Debug\net9.0\TestAssembly.dll");
        mockDebuggerWrapper.FileSystem.Returns(new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { Path.Combine(@"C:\SolutionDirectory", $"{Constants.SolutionName}.sln"), new MockFileData(string.Empty) }
        }));
        DevUtilities.SetDebuggerWrapper(mockDebuggerWrapper);

        // Act
        var result = DevUtilities.GetSolutionDirectory(folder);

        // Assert
        result.FullName.Should().Be(expectedPath);
    }

    [Fact]
    public void GetSolutionDirectory_ShouldReturnCorrectPath_WhenDebuggerIsNotAttached()
    {
        // Arrange
        var folder = "TestFolder";
        var expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Model",
            folder);
        var mockDebuggerWrapper = Substitute.For<IDebuggerWrapper>();
        mockDebuggerWrapper.IsAttached.Returns(false);
        DevUtilities.SetDebuggerWrapper(mockDebuggerWrapper);

        // Act
        var result = DevUtilities.GetSolutionDirectory(folder);

        // Assert
        result.FullName.Should().Be(expectedPath);
    }
}