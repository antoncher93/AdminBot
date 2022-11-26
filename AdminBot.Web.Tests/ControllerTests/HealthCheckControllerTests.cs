using System.Net;
using FluentAssertions;
using Xunit;

namespace AdminBot.Web.Tests.ControllerTests;

public class HealthCheckControllerTests
{
    [Fact]
    public async Task ReturnsOk()
    {
        var sut = SutFactory.Create();

        var response = await sut.GetHealthCheckAsync();

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }
}