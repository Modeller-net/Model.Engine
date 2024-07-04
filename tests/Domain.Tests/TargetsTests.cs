using FluentAssertions;

namespace Domain.Tests;

public class TargetsTests
{
    private readonly Targets _targets = new Targets();

    [Fact]
    public void Shared_ShouldReturnSameInstance()
    {
        var shared1 = Targets.Shared;
        var shared2 = Targets.Shared;

        shared1.Should().BeSameAs(shared2);
    }

    [Fact]
    public void Default_ShouldReturnLastSystemTarget()
    {
        var defaultTarget = Targets.Default;

        defaultTarget.Should().Be("net9.0");
    }

    [Fact]
    public void Supported_ShouldReturnSystemTargetsInitially()
    {
        var supportedTargets = _targets.Supported;

        supportedTargets.Should().BeEquivalentTo(new[] { "net6.0", "net7.0", "net8.0", "net9.0" });
    }

    [Fact]
    public void RegisterTarget_ShouldAddNewTarget()
    {
        _targets.RegisterTarget("net10.0");

        _targets.Supported.Should().Contain("net10.0");
    }

    [Fact]
    public void RegisterTarget_ShouldNotAddDuplicateTarget()
    {
        _targets.RegisterTarget("net10.0");
        _targets.RegisterTarget("net10.0");

        _targets.Supported.Count(t => t == "net10.0").Should().Be(1);
    }

    [Fact]
    public void Reset_ShouldReturnToSystemTargets()
    {
        _targets.RegisterTarget("net10.0");
        _targets.Reset();

        _targets.Supported.Should().BeEquivalentTo(new[] { "net6.0", "net7.0", "net8.0", "net9.0" });
    }
}