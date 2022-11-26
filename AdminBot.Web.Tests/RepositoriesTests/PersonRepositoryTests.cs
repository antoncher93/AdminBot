using AdminBot.Common;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

namespace AdminBot.Web.Tests.RepositoriesTests;

public class PersonRepositoryTests
{
    [Fact]
    public async Task AddsPerson()
    {
        var sut = SutFactory.Create();

        var dateTime = DateTime.UtcNow;

        var person = await sut.AddPersonAsync(
            userId: Gen.RandomLong(),
            chatId: Gen.RandomLong(),
            userName: Gen.RandomString(),
            createdAt: dateTime);

        var record = await sut.FindPersonRecordAsync(
            userId: person.UserId,
            chatId: person.ChatId);

        var actual = MapPersonFromRecord(record);

        actual.Should()
            .BeEquivalentTo(
                person,
                options => options
                    .Excluding(p => p.Id)
                    .Using<DateTime>(ctx
                        => ctx.Subject
                            .Should()
                            .BeCloseTo(ctx.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task FindsExistingPerson()
    {
        var person = ObjectsGen.CreateRandomPerson();

        var sut = SutFactory.Create();

        await sut.InsertPersonRecordAsync(person);

        var actual = await sut.FindPersonAsync(
            userId: person.UserId,
            chatId: person.ChatId);

        actual.Should()
            .BeEquivalentTo(
                person,
                options => options
                    .Excluding(p => p.Id)
                    .Using<DateTime>(ctx
                        => ctx.Subject
                            .Should()
                            .BeCloseTo(ctx.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task IncrementsWarns()
    {
        var sut = SutFactory.Create();

        var dateTime = DateTime.UtcNow;

        var person = await sut.AddPersonAsync(
            userId: Gen.RandomLong(),
            chatId: Gen.RandomLong(),
            userName: Gen.RandomString(),
            createdAt: dateTime - TimeSpan.FromDays(Gen.RandomInt(1, 5)));

        await sut.IncrementWarnsAsync(
            person: person,
            datetime: dateTime);

        var actualPerson = await sut.FindPersonRecordAsync(
            userId: person.UserId,
            chatId: person.ChatId);

        actualPerson.Warns
            .Should()
            .Be(person.Warns + 1);
    }
    
    private static Person MapPersonFromRecord(PersonRecord record)
    {
        return new Person(
            id: record.Id,
            userId: record.UserId,
            chatId: record.ChatId,
            username: record.Username,
            createdAt: record.CreatedAt,
            warns: record.Warns,
            updatedAt: record.UpdatedAt);
    }
}