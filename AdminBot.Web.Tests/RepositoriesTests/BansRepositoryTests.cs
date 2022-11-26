using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

namespace AdminBot.Web.Tests.RepositoriesTests;

public class BansRepositoryTests
{
    [Fact]
    public async Task SavesBan()
    {
        var sut = SutFactory.Create();

        var person = ObjectsGen.CreateRandomPerson();

        var ban = ObjectsGen.CreateRandomBan(person);

        await sut.SaveBan(ban);

        var expectedRecord = new BanRecord(
            personId: ban.Person.Id,
            id: 0,
            createdAt: ban.CreatedAt,
            expireAt: ban.ExpireAt);

        var actualRecord = await sut.SearchBanRecordByPersonId(person.Id);

        actualRecord.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(
                expectation: expectedRecord,
                config: options => options
                    .Excluding(record => record.Id)
                    .Using<DateTime>(ctx
                        => ctx.Subject
                            .Should()
                            .BeCloseTo(ctx.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task FindsExistingBan()
    {
        var sut = SutFactory.Create();

        var person = ObjectsGen.CreateRandomPerson();

        var ban = ObjectsGen.CreateRandomBan(person);

        await sut.SaveBanRecordAsync(ban);

        var actual = await sut.FindBanForPersonAsync(person);
        
        actual.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(
                expectation: ban,
                config: options => options
                    .Using<DateTime>(ctx
                        => ctx.Subject
                            .Should()
                            .BeCloseTo(ctx.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());
    }
}