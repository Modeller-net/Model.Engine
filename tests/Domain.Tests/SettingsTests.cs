using FluentAssertions;
using Names;

namespace Domain.Tests;

public class SettingsTests
{
    [Fact]
    public void ShouldReturnDefaultSettings()
    {
        var defaultSettings = Settings.Default;

        Settings.DefaultTemplateFolder.FullName.Should().Be(defaultSettings.TemplateFolder);
        Settings.DefaultDefinitionFolder.FullName.Should().Be(defaultSettings.DefinitionFolder);
        Settings.DefaultOutputFolder.FullName.Should().Be(defaultSettings.OutputFolder);
        Targets.Default.Should().Be(defaultSettings.Target);
    }

    [Fact]
    public void ShouldReturnDefaultTemplateFolder()
    {
        var defaultTemplateFolder = Settings.DefaultTemplateFolder;

        defaultTemplateFolder.Should().BeOfType<DirectoryInfo>();
    }

    [Fact]
    public void ShouldReturnDefaultDefinitionFolder()
    {
        var defaultDefinitionFolder = Settings.DefaultDefinitionFolder;

        defaultDefinitionFolder.Should().BeOfType<DirectoryInfo>();
    }

    [Fact]
    public void ShouldReturnDefaultOutputFolder()
    {
        var defaultOutputFolder = Settings.DefaultOutputFolder;

        defaultOutputFolder.Should().BeOfType<DirectoryInfo>();
    }

    [Fact]
    public void ShouldSetPropertiesCorrectly()
    {
        var settings = new Settings("Template", "Definition", "Output", "Target")
        {
            Enterprise = new Enterprise("TestComany", NameType.FromString("TestProject"), new("TestSummary")),
            DomainService = new Service(NameType.FromString("TestService"), new NonEmptyString("Service summary")),
            Entity = Entity.Create(NameType.FromString("TestEntity"), new NonEmptyString("Entity summary"),[]),
            TemplateName = "TemplateName",
            DefinitionName = "DefinitionName",
            SupportRegen = false,
            Version = "2.0.0",
            Packages = new List<Package> { new Package() }
        };

        settings.TemplateFolder.Should().Be("Template");
        settings.DefinitionFolder.Should().Be("Definition");
        settings.OutputFolder.Should().Be("Output");
        settings.Target.Should().Be("Target");
        settings.Enterprise.Should().NotBeNull();
        settings.DomainService.Should().NotBeNull();
        settings.Entity.Should().NotBeNull();
        settings.TemplateName.Should().Be("TemplateName");
        settings.DefinitionName.Should().Be("DefinitionName");
        settings.SupportRegen.Should().BeFalse();
        settings.Version.Should().Be("2.0.0");
        settings.Packages.Should().HaveCount(1);
    }
}