using System.Net;
using FluentAssertions;
using Xunit;

namespace AdminBot.Web.Tests.ControllerTests;

public class UpdateControllerTests
{
    [Fact]
    public async Task ReturnsOk()
    {
        var sut = SutFactory.Create();

        var update = ObjectsGen.CreateMessageUpdate(
            text: Gen.RandomString(),
            chatId: Gen.RandomLong(),
            messageId: Gen.RandomInt(),
            from: ObjectsGen.CreateUser(
                userId: Gen.RandomLong(),
                username: Gen.RandomString()));

        var response = await sut.PostUpdateAsync(update);

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task ReturnsOkForJson()
    {
        var sut = SutFactory.Create();
        
        var jsonContent = await File.ReadAllTextAsync("update.json");

        var response = await sut.PostJsonUpdateAsync(jsonContent);

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }
}