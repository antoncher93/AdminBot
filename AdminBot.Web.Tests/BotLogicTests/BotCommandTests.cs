using AdminBot.Common;
using AdminBot.Common.Messages;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

namespace AdminBot.Web.Tests.BotLogicTests;

public class BotCommandTests
{
    [Fact]
    public async Task WarnsBotCommandSendsMessage()
    {
        var sut = SutFactory.Create();

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

        await sut.HandleUpdateAsync(warnMessageUpdate);

        var expectedMessage = new WarnPersonMessage(
            blameMessageId: messageForWarn.MessageId,
            userName: user.Username ?? user.FirstName,
            userId: user.Id,
            warns: 1,
            warnsLimit: sut.DefaultWarnsLimit);
        
        var actualMessages = sut.GetBotMessages(chatId);

        actualMessages
            .Single()
            .Should()
            .BeEquivalentTo(expectedMessage);

        sut.GetDeletedMessages(chatId)
            .Single()
            .Should()
            .Be(warnMessageUpdate.Message.MessageId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("TestUserName")]
    public async Task WarnBotCommandRestrictsPersonWhenWarnLimitExceeded(
        string userName)
    {
        var sut = SutFactory.Create();

        var dateTime = DateTime.Today;
        var warns = Gen.RandomInt(5);
        var chatId = Gen.RandomLong();

        var user = ObjectsGen.CreateUser(
            userId: Gen.RandomLong(),
            username: userName);

        var messageForWarn = ObjectsGen.CreateMessage(
            messageId: Gen.RandomInt(),
            text: Gen.RandomString(),
            chatId: chatId,
            from: user);
        
        var admin = ObjectsGen.CreateUser(
            userId: Gen.RandomLong(),
            username: Gen.RandomString());
        
        sut.AddChatAdmin(admin, chatId);

        await sut.SetupPersonAsync(
            userId: user.Id,
            chatId: chatId,
            userName: user.Username,
            firstName: user.FirstName,
            warns: warns,
            createdAt: dateTime,
            updatedAt: dateTime);

        await sut.SetChatSettingsAsync(
            chatId: chatId,
            agreement: Gen.RandomString(),
            warnsLimit: warns,
            banTtl: Gen.RandomTimeSpan());

        var warnMessageUpdate = ObjectsGen.CreateMessageUpdate(
            messageId: Gen.RandomInt(),
            text: "/warn",
            chatId: chatId,
            from: admin,
            replyToMessage: messageForWarn);

        await sut.HandleUpdateAsync(warnMessageUpdate);

        sut.GetRestrictedUsers(chatId)
            .Single()
            .Should()
            .Be(user.Id);
    }
    
    [Theory]
    [InlineData("/ban")]
    [InlineData("/ban@test_bot")]
    public async Task BanCommandRestrictsUser(
        string command)
    {
        var sut = SutFactory.Create(
            botName: "test_bot");

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
            text: command,
            chatId: chatId,
            from: admin,
            replyToMessage: messageForBan);

        await sut.HandleUpdateAsync(warnMessageUpdate);

        sut.GetRestrictedUsers(chatId)
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(user.Id);
    }
    
    [Theory]
    [InlineData("/ban")]
    [InlineData("/ban@test_bot")]
    public async Task BanCommandProcessMessages(
        string command)
    {
        var sut = SutFactory.Create(
            botName: "test_bot");

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
            text: command,
            chatId: chatId,
            from: admin,
            replyToMessage: messageForBan);

        await sut.HandleUpdateAsync(warnMessageUpdate);

        var actualMessages = sut.GetBotMessages(chatId);

        actualMessages
            .Single()
            .Should()
            .BeEquivalentTo(
                new BanPersonMessage(
                    blameMessageId: messageForBan.MessageId,
                    userName: user.Username ?? user.FirstName,
                    userId: user.Id,
                    expireAt: sut.GetProvidedDateTime() + sut.DefaultBanTtl));

        sut.GetDeletedMessages(chatId)
            .Single()
            .Should()
            .Be(warnMessageUpdate.Message.MessageId);
    }

    [Theory]
    [InlineData("/setAgreement")]
    [InlineData("/setAgreement@test_bot")]
    public async Task SetChatAgreementCreatesSettings(
        string command)
    {
        var sut = SutFactory.Create(
            botName: "test_bot");

        var dateTime = sut.GetProvidedDateTime();

        var chatId = Gen.RandomLong();
        
        var admin = ObjectsGen.CreateUser(
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
            text: command,
            chatId: chatId,
            from: admin,
            replyToMessage: messageWithAgreement);

        await sut.HandleUpdateAsync(warnMessageUpdate);
        
        sut.AssertChatMessages(
            chatId: chatId,
            new ChatRulesHasBeenChangedMessage(
                agreement: agreement));

        var expectedChatSettings = new ChatSettings(
            telegramId: chatId,
            agreement: agreement,
            warnsLimit: sut.DefaultWarnsLimit,
            banTtl: sut.DefaultBanTtl,
            createdAt: dateTime);

        var actual = await sut.FindChatAgreementAsync(chatId);

        actual.Should()
            .BeEquivalentTo(expectedChatSettings);
    }
    
    [Theory]
    [InlineData("/setAgreement")]
    [InlineData("/setAgreement@test_bot")]
    public async Task SetChatAgreementSendsMessage(
        string command)
    {
        var sut = SutFactory.Create(
            botName: "test_bot");

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
            text: command,
            chatId: chatId,
            from: admin,
            replyToMessage: messageWithAgreement);

        await sut.HandleUpdateAsync(warnMessageUpdate);
        
        sut.AssertChatMessages(
            chatId: chatId,
            new ChatRulesHasBeenChangedMessage(
                agreement: agreement));

        sut.GetDeletedMessages(chatId)
            .Single()
            .Should()
            .Be(warnMessageUpdate.Message.MessageId);
    }
    
    [Theory]
    [InlineData("/setAgreement")]
    [InlineData("/setAgreement@test_bot")]
    public async Task SetChatAgreementUpdatesAgreement(
        string command)
    {
        var sut = SutFactory.Create(
            botName: "test_bot");

        var chatId = Gen.RandomLong();
        
        var admin = ObjectsGen.CreateUser(
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
            text: command,
            chatId: chatId,
            from: admin,
            replyToMessage: messageWithAgreement);

        var dateTime = sut.GetProvidedDateTime();
        
        await sut.HandleUpdateAsync(warnMessageUpdate);
        
        var expectedChatAgreement = new ChatSettings(
            telegramId: chatId,
            agreement: agreement1,
            warnsLimit: sut.DefaultWarnsLimit,
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
            text: command,
            chatId: chatId,
            from: admin,
            replyToMessage: messageWithAgreement2);

        var dateTime2 = sut.GetProvidedDateTime();
        
        await sut.HandleUpdateAsync(warnMessageUpdate2);
        
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