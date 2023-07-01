using AdminBot.Common;
using AdminBot.Common.Messages;
using AdminBot.UseCases.Infrastructure.Extensions;
using AdminBot.Web.Tests.Fakes;
using FluentAssertions;
using FluentAssertions.Extensions;
using Telegram.Bot.Types;
using Xunit;

namespace AdminBot.Web.Tests.BotLogicTests;

public class BotCommandTests
{
    [Fact]
    public async Task WarnsBotCommandTest()
    {
        var sut = SutFactory.Create();

        var chatId = Gen.RandomLong();

        var user = sut.CreateChatMember(chatId);

        var messageForWarn = ObjectsGen.CreateRandomMessage(
            text: Gen.RandomString(),
            chatId: chatId,
            from: user);

        var admin = sut.CreateChatAdmin(
            chatId: chatId);

        var warnCommandMessage = ObjectsGen.CreateRandomMessage(
            text: "/warn",
            chatId: chatId,
            from: admin,
            replyToMessage: messageForWarn);

        var warnMessageUpdate = ObjectsGen.CreateMessageUpdate(warnCommandMessage);

        await sut.HandleUpdateAsync(warnMessageUpdate);
        
        sut.AssertBotActions(
            new BotActions.TextMessage(
                ChatId: chatId,
                ReplyToMessageId: messageForWarn.MessageId,
                Text: $"Предупреждение 1! " +
                      $"Получите больше {sut.DefaultWarnsLimit} и будете забанены!"),
            new BotActions.DeleteMessage(
                ChatId: chatId,
                MessageId: warnCommandMessage.MessageId));
    }

    [Fact]
    public async Task WarnBotCommandRestrictsPersonWhenWarnLimitExceeded()
    {
        var sut = SutFactory.Create();

        var dateTime = DateTime.Today;
        var warns = Gen.RandomInt(5);
        var chatId = Gen.RandomLong();
        var banTtl = Gen.RandomTimeSpan();
        var user = sut.CreateChatMember(chatId);
        var admin = sut.CreateChatAdmin(chatId);
        
        var messageForWarn = ObjectsGen.CreateRandomMessage(
            messageId: Gen.RandomInt(),
            text: Gen.RandomString(),
            chatId: chatId,
            from: user);

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
            banTtl: banTtl);

        var warnMessage = ObjectsGen.CreateRandomMessage(
            text: "/warn",
            chatId: chatId,
            from: admin,
            replyToMessage: messageForWarn);

        var warnMessageUpdate = ObjectsGen.CreateMessageUpdate(warnMessage);

        await sut.HandleUpdateAsync(warnMessageUpdate);

        var restrictedUntilDateTime = sut.GetUtcNow() + banTtl;
        
        sut.AssertBotActions(
            new BotActions.RestrictChatMember(
                ChatId: chatId,
                UserId: user.Id,
                CanSendMessages: false,
                CanSendMediaMessages: false,
                CanSendOtherMessages: false,
                UntilDateTime: restrictedUntilDateTime.RoundToSeconds()),
            new BotActions.TextMessage(
                ChatId: chatId,
                ReplyToMessageId: messageForWarn.MessageId,
                Text: $"{CreateUserMention(user)} забанен до {ToMoscowTime(restrictedUntilDateTime):g}!"),
            new BotActions.DeleteMessage(
                MessageId: warnMessage.MessageId,
                ChatId: chatId));
    }
    
    [Theory]
    [InlineData("/ban")]
    [InlineData("/ban@test_bot")]
    public async Task BanCommandTest(
        string command)
    {
        var sut = SutFactory.Create(
            botName: "test_bot");

        var chatId = Gen.RandomLong();
        var user = sut.CreateChatAdmin(chatId);
        var admin = sut.CreateChatAdmin(chatId);

        var messageForBan = ObjectsGen.CreateRandomMessage(
            text: Gen.RandomString(),
            chatId: chatId,
            from: user);

        var botCommandMessage = ObjectsGen.CreateRandomMessage(
            chatId: chatId,
            from: admin,
            text: command,
            replyToMessage: messageForBan);

        var warnMessageUpdate = ObjectsGen.CreateMessageUpdate(botCommandMessage);
        
        var restrictedUntilDateTime = sut.GetUtcNow() + sut.DefaultBanTtl;

        await sut.HandleUpdateAsync(warnMessageUpdate);

        sut.AssertBotActions(
            new BotActions.RestrictChatMember(
                ChatId: chatId,
                UserId: user.Id,
                CanSendMessages: false,
                CanSendMediaMessages: false,
                CanSendOtherMessages: false,
                UntilDateTime: restrictedUntilDateTime.RoundToSeconds()),
            new BotActions.TextMessage(
                ChatId: chatId,
                ReplyToMessageId: messageForBan.MessageId,
                Text: $"{CreateUserMention(user)} забанен до {ToMoscowTime(restrictedUntilDateTime):g}!"),
            new BotActions.DeleteMessage(
                MessageId: botCommandMessage.MessageId,
                ChatId: chatId));
    }

    [Theory]
    [InlineData("/setAgreement")]
    [InlineData("/setAgreement@test_bot")]
    public async Task SetChatAgreementSendsMessageTest(
        string command)
    {
        var sut = SutFactory.Create(
            botName: "test_bot");

        var dateTime = sut.GetUtcNow();
        var chatId = Gen.RandomLong();
        var admin = sut.CreateChatAdmin(chatId);
        var agreement = Gen.RandomString();
        
        var messageWithAgreement = ObjectsGen.CreateRandomMessage(
            messageId: Gen.RandomInt(),
            text: agreement,
            chatId: chatId,
            from: admin);

        var botCommandMessage = ObjectsGen.CreateRandomMessage(
            text: command,
            chatId: chatId,
            from: admin,
            replyToMessage: messageWithAgreement);

        var warnMessageUpdate = ObjectsGen.CreateMessageUpdate(botCommandMessage);

        await sut.HandleUpdateAsync(warnMessageUpdate);
        
        sut.AssertBotActions(
            new BotActions.TextMessage(
                ChatId: chatId,
                Text: $"Правила чата были изменены!"),
            new BotActions.DeleteMessage(
                ChatId: chatId,
                MessageId: botCommandMessage.MessageId));
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
        var admin = sut.CreateChatAdmin(chatId);
        var agreement1 = Gen.RandomString();
        
        var messageWithAgreement = ObjectsGen.CreateRandomMessage(
            messageId: Gen.RandomInt(),
            text: agreement1,
            chatId: chatId,
            from: admin);

        var warnMessageUpdate = ObjectsGen.CreateMessageUpdate(
            text: command,
            chatId: chatId,
            from: admin,
            replyToMessage: messageWithAgreement);

        var dateTime = sut.GetUtcNow();
        
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
        
        var messageWithAgreement2 = ObjectsGen.CreateRandomMessage(
            messageId: Gen.RandomInt(),
            text: agreement2,
            chatId: chatId,
            from: admin);

        var warnMessageUpdate2 = ObjectsGen.CreateMessageUpdate(
            text: command,
            chatId: chatId,
            from: admin,
            replyToMessage: messageWithAgreement2);

        var dateTime2 = sut.GetUtcNow();
        
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
    
    private static string CreateUserMention(
        User user)
    {
        var mention = user.Username ?? user.FirstName;
        return $"[{mention}](tg://user?{user.Id})";
    }

    private static DateTime ToMoscowTime(DateTime utcTime)
    {
        return utcTime + TimeSpan.FromHours(3);
    }
}