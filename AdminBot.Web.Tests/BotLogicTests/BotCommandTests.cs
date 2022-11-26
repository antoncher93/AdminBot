using AdminBot.Common;
using AdminBot.Common.Messages;
using AdminBot.Web.Tests.BotLogicTests;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

namespace AdminBot.Web.Tests.BotCommandsTests;

public class BotCommandTests
{
    [Fact]
    public async Task WarnsCommandSendsMessage()
    {
        var sut = SutFactory.Create();

        var dateTime = DateTime.UtcNow;

        var chatId = Gen.RandomLong();

        var user = ObjectsGen.CreateUser(
            userId: Gen.RandomLong(),
            username: Gen.RandomString());

        var messageForWarn = ObjectsGen.CreateMessage(
            messageId: Gen.RandomInt(),
            text: Gen.RandomString(),
            chatId: chatId,
            from: user);
        
        var admin = ObjectsGen.CreateUser(
            userId: Gen.RandomLong(),
            username: Gen.RandomString());
        
        sut.AddChatAdmin(admin, chatId);

        var warnMessageUpdate = ObjectsGen.CreateMessageUpdate(
            messageId: Gen.RandomInt(),
            text: "/warn",
            chatId: chatId,
            from: admin,
            replyToMessage: messageForWarn);

        await sut.HandleUpdateAsync(warnMessageUpdate, dateTime);

        var expectedPerson = new Person(
            id: Gen.RandomInt(),
            userId: user.Id,
            username: user.Username,
            chatId: chatId,
            createdAt: dateTime,
            updatedAt: dateTime,
            warns: 1);
        
        var actualMessages = sut.GetBotMessages(chatId);

        actualMessages
            .Should()
            .HaveCount(1);

        actualMessages[0]
            .Should()
            .BeOfType<WarnPersonMessage>();

        var actualMessage = actualMessages[0] as WarnPersonMessage;

        actualMessage.Person
            .Should()
            .BeEquivalentTo(
                expectation: expectedPerson,
                config: options => options
                    .Excluding(person => person.Id)
                    .Using<DateTime>(ctx
                            => ctx.Subject
                                .Should()
                                .BeCloseTo(ctx.Expectation, 1.Seconds()))
                        .WhenTypeIs<DateTime>());

        var actualPerson = await sut.FindPerson(user.Id, chatId);

        actualPerson.Should()
            .NotBeNull();

        actualPerson.Warns.Should().Be(1);
    }

    [Fact]
    public async Task WarnRestrictsPersonWhenWarnLimitExceeded()
    {
        var sut = SutFactory.Create();

        var dateTime = DateTime.UtcNow;

        var chatId = Gen.RandomLong();

        var user = ObjectsGen.CreateUser(
            userId: Gen.RandomLong(),
            username: Gen.RandomString());

        var person = ObjectsGen.CreateRandomPerson();

        var messageForWarn = ObjectsGen.CreateMessage(
            messageId: Gen.RandomInt(),
            text: Gen.RandomString(),
            chatId: chatId,
            from: user);
        
        var admin = ObjectsGen.CreateUser(
            userId: Gen.RandomLong(),
            username: Gen.RandomString());
        
        sut.AddChatAdmin(admin, chatId);

        var warnMessageUpdate = ObjectsGen.CreateMessageUpdate(
            messageId: Gen.RandomInt(),
            text: "/warn",
            chatId: chatId,
            from: admin,
            replyToMessage: messageForWarn);

        await sut.HandleUpdateAsync(warnMessageUpdate, dateTime);
    }
    
    [Fact]
    public async Task BanCommandRestrictsUser()
    {
        var sut = SutFactory.Create();

        var chatId = Gen.RandomLong();

        var user = ObjectsGen.CreateUser(
            userId: Gen.RandomLong(),
            username: Gen.RandomString());
        
        var admin = ObjectsGen.CreateUser(
            userId: Gen.RandomLong(),
            username: Gen.RandomString());

        sut.AddChatAdmin(admin, chatId);

        var messageForBan = ObjectsGen.CreateMessage(
            messageId: Gen.RandomInt(),
            text: Gen.RandomString(),
            chatId: chatId,
            from: user);

        var warnMessageUpdate = ObjectsGen.CreateMessageUpdate(
            messageId: Gen.RandomInt(),
            text: "/ban",
            chatId: chatId,
            from: admin,
            replyToMessage: messageForBan);

        var dateTime = DateTime.UtcNow;
        
        await sut.HandleUpdateAsync(warnMessageUpdate, dateTime);

        var expiredAt = dateTime + sut.DefaultBanTtl;

        var actualMessages = sut.GetBotMessages(chatId);

        actualMessages
            .Should()
            .HaveCount(1);

        var actualMessage = actualMessages[0] as BanPersonMessage;

        var expectedPerson = new Person(
            id: Gen.RandomInt(),
            userId: user.Id,
            chatId: chatId,
            username: user.Username,
            warns: 0,
            createdAt: dateTime,
            updatedAt: dateTime);

        actualMessage.ExpireAt
            .Should()
            .BeCloseTo(expiredAt, TimeSpan.FromSeconds(1));

        actualMessage.Person
            .Should()
            .BeEquivalentTo(
                expectation: expectedPerson,
                config: options => options
                    .Excluding(person => person.Id)
                    .Using<DateTime>(ctx
                        => ctx.Subject
                            .Should()
                            .BeCloseTo(ctx.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());

        sut.GetRestrictedUsers(chatId)
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(user.Id);
    }

    [Fact]
    public async Task SetChatAgreementCreatesSettings()
    {
        var sut = SutFactory.Create();

        var chatId = Gen.RandomLong();
        
        var admin =    ObjectsGen.CreateUser(
            userId: Gen.RandomLong(),
            username: Gen.RandomString());
        
        sut.AddChatAdmin(admin, chatId);

        var agreement = Gen.RandomString();
        
        var messageWithAgreement = ObjectsGen.CreateMessage(
            messageId: Gen.RandomInt(),
            text: agreement,
            chatId: chatId,
            from: admin);

        var warnMessageUpdate = ObjectsGen.CreateMessageUpdate(
            messageId: Gen.RandomInt(),
            text: "/setAgreement",
            chatId: chatId,
            from: admin,
            replyToMessage: messageWithAgreement);

        var dateTime = DateTime.UtcNow;
        
        await sut.HandleUpdateAsync(warnMessageUpdate, dateTime);
        
        sut.AssertChatMessages(
            chatId: chatId,
            new ChatRulesHasBeenChangedMessage(
                agreement: agreement));

        var expectedChatAgreement = new ChatSettings(
            telegramId: chatId,
            agreement: agreement,
            warnsLimit: 2,
            banTtl: sut.DefaultBanTtl,
            createdAt: dateTime);

        var actual = await sut.FindChatAgreementAsync(chatId);

        actual.Should()
            .BeEquivalentTo(
                expectedChatAgreement,
                config: options=> options
                    .Using<DateTime>(ctx
                        => ctx.Subject
                            .Should()
                            .BeCloseTo(ctx.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());
    }
    
    [Fact]
    public async Task SetChatAgreementSendsMessage()
    {
        var sut = SutFactory.Create();

        var chatId = Gen.RandomLong();
        
        var admin =    ObjectsGen.CreateUser(
            userId: Gen.RandomLong(),
            username: Gen.RandomString());
        
        sut.AddChatAdmin(admin, chatId);

        var agreement = Gen.RandomString();
        
        var messageWithAgreement = ObjectsGen.CreateMessage(
            messageId: Gen.RandomInt(),
            text: agreement,
            chatId: chatId,
            from: admin);

        var warnMessageUpdate = ObjectsGen.CreateMessageUpdate(
            messageId: Gen.RandomInt(),
            text: "/setAgreement",
            chatId: chatId,
            from: admin,
            replyToMessage: messageWithAgreement);

        var dateTime = DateTime.UtcNow;
        
        await sut.HandleUpdateAsync(warnMessageUpdate, dateTime);
        
        sut.AssertChatMessages(
            chatId: chatId,
            new ChatRulesHasBeenChangedMessage(
                agreement: agreement));
    }
    
    [Fact]
    public async Task SetChatAgreementUpdatesAgreement()
    {
        var sut = SutFactory.Create();

        var chatId = Gen.RandomLong();
        
        var admin =    ObjectsGen.CreateUser(
            userId: Gen.RandomLong(),
            username: Gen.RandomString());
        
        sut.AddChatAdmin(admin, chatId);

        var agreement1 = Gen.RandomString();
        
        var messageWithAgreement = ObjectsGen.CreateMessage(
            messageId: Gen.RandomInt(),
            text: agreement1,
            chatId: chatId,
            from: admin);

        var warnMessageUpdate = ObjectsGen.CreateMessageUpdate(
            messageId: Gen.RandomInt(),
            text: "/setAgreement",
            chatId: chatId,
            from: admin,
            replyToMessage: messageWithAgreement);

        var dateTime = DateTime.UtcNow;
        
        await sut.HandleUpdateAsync(warnMessageUpdate, dateTime);
        
        var expectedChatAgreement = new ChatSettings(
            telegramId: chatId,
            agreement: agreement1,
            warnsLimit: 2,
            banTtl: sut.DefaultBanTtl,
            createdAt: dateTime);

        var actual1 = await sut.FindChatAgreementAsync(chatId);

        actual1.Should()
            .BeEquivalentTo(
                expectedChatAgreement,
                config: options=> options
                    .Using<DateTime>(ctx
                        => ctx.Subject
                            .Should()
                            .BeCloseTo(ctx.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());
        
        var agreement2 = Gen.RandomString();
        
        var messageWithAgreement2 = ObjectsGen.CreateMessage(
            messageId: Gen.RandomInt(),
            text: agreement2,
            chatId: chatId,
            from: admin);

        var warnMessageUpdate2 = ObjectsGen.CreateMessageUpdate(
            messageId: Gen.RandomInt(),
            text: "/setAgreement",
            chatId: chatId,
            from: admin,
            replyToMessage: messageWithAgreement2);

        var dateTime2 = DateTime.UtcNow;
        
        await sut.HandleUpdateAsync(warnMessageUpdate2, dateTime2);
        
        var expectedChatAgreement2 = new ChatSettings(
            telegramId: chatId,
            agreement: agreement2,
            warnsLimit: sut.DefaultWarnsLimit,
            banTtl: sut.DefaultBanTtl,
            createdAt: dateTime2);

        var actual2 = await sut.FindChatAgreementAsync(chatId);
        
        actual2.Should()
            .BeEquivalentTo(
                expectedChatAgreement2,
                config: options=> options
                    .Using<DateTime>(ctx
                        => ctx.Subject
                            .Should()
                            .BeCloseTo(ctx.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());
    }
}