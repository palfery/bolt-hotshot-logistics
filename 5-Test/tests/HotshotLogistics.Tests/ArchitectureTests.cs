using NetArchTest.Rules;
using Xunit;
using HotshotLogistics.Domain.Models;
using HotshotLogistics.Application.Services;
using HotshotLogistics.Data.Repositories;

namespace HotshotLogistics.Tests;

public class ArchitectureTests
{
    private const string Presentation = "HotshotLogistics.Api";
    private const string Infrastructure = "HotshotLogistics.Infrastructure";
    private const string Application = "HotshotLogistics.Application";
    private const string Data = "HotshotLogistics.Data";

    [Fact]
    public void Domain_should_not_depend_on_other_layers()
    {
        var result = Types.InAssembly(typeof(Driver).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(Application, Data, Infrastructure, Presentation)
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join(',', result.FailingTypeNames));
    }

    [Fact]
    public void Application_should_not_depend_on_presentation()
    {
        var result = Types.InAssembly(typeof(DriverService).Assembly)
            .ShouldNot()
            .HaveDependencyOn(Presentation)
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join(',', result.FailingTypeNames));
    }

    [Fact]
    public void Repositories_should_be_internal()
    {
        var result = Types.InAssembly(typeof(DriverRepository).Assembly)
            .That().HaveNameEndingWith("Repository")
            .Should().NotBePublic()
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join(',', result.FailingTypeNames));
    }
}
