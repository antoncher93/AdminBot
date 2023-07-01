using AdminBot.Common.CallbackQueries;
using AdminBot.Common.Messages;
using AdminBot.Web.Tests.Fakes;
using FluentAssertions;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Xunit;

namespace AdminBot.Web.Tests.BotLogicTests;

public class MessageHandlerTests
{
    [Fact]
    public async Task RestrictsNewChatMembers()
    {
        var sut = SutFactory.Create();

        var chatId = Gen.RandomLong();

        var newUser = ObjectsGen.CreateUser();
        
        var newChatMemberMessage = ObjectsGen.CreateRandomMessage(
            chatId: chatId,
            from: null,
            newChatMembers: new[] { newUser });

        var update = ObjectsGen.CreateMessageUpdate(newChatMemberMessage);
        
        var agreement = Gen.RandomString();

        await sut.SetChatSettingsAsync(
            chatId: chatId,
            agreement: agreement,
            warnsLimit: Gen.RandomInt(10),
            banTtl: Gen.RandomTimeSpan());

        await sut.HandleUpdateAsync(update);

        var mention = CreateUserMention(newUser);
        
        sut.AssertBotActions(
            boActions: new BotActions.TextMessage(
                Text: $"{mention} чтобы писать сообщения нужно принять правила группы!\n" + agreement,
                ChatId: chatId));
    }

    [Fact]
    public async Task RemovesRestrictionOnAcceptChatRules()
    {
        var sut = SutFactory.Create();

        var chatId = Gen.RandomLong();

        var user = ObjectsGen.CreateUser(
            userId: Gen.RandomLong(),
            username: Gen.RandomString());
        
        var newChatMemberMessage = ObjectsGen.CreateRandomMessage(
            messageId: Gen.RandomInt(),
            chatId: chatId,
            from: null,
            newChatMembers: new[]{ user });

        var update = ObjectsGen.CreateMessageUpdate(newChatMemberMessage);

        await sut.SetChatSettingsAsync(
            chatId: chatId,
            agreement: Gen.RandomString(),
            warnsLimit: Gen.RandomInt(),
            banTtl: Gen.RandomTimeSpan());

        await sut.HandleUpdateAsync(update);
        
        var acceptChatRulesEnvelope = CallbackQueryEnvelope.FromAcceptChatRules(
            new AcceptChatRulesCallbackQuery(
                userId: user.Id));

        var data = JsonConvert.SerializeObject(acceptChatRulesEnvelope);

        var messageId = Gen.RandomInt();

        var callbackQueryUpdate = ObjectsGen.CreateCallbackQueryUpdate(
            from: user,
            callbackQueryData: data,
            messageId: messageId,
            chatId: chatId);

        await sut.HandleUpdateAsync(
            update: callbackQueryUpdate);
        
        sut.AssertBotActions(
            new BotActions.RestrictChatMember(
                ChatId: chatId,
                UserId: user.Id),
            new BotActions.RestrictChatMember(
                ChatId: chatId,
                UserId: user.Id,
                CanSendMessages: true,
                CanSendMediaMessages: true,
                CanSendOtherMessages: true));
    }

    [Fact]
    public async Task RemovesMessageOnAcceptClicked()
    {
        var sut = SutFactory.Create();

        var chatId = Gen.RandomLong();

        var user = ObjectsGen.CreateUser(
            userId: Gen.RandomLong(),
            username: Gen.RandomString());
        
        var acceptChatRulesEnvelope = CallbackQueryEnvelope.FromAcceptChatRules(
            new AcceptChatRulesCallbackQuery(
                userId: user.Id));

        var data = JsonConvert.SerializeObject(acceptChatRulesEnvelope);

        var messageId = Gen.RandomInt();

        var callbackQueryUpdate = ObjectsGen.CreateCallbackQueryUpdate(
            from: user,
            callbackQueryData: data,
            messageId: messageId,
            chatId: chatId);

        await sut.HandleUpdateAsync(
            update: callbackQueryUpdate);
        
        sut.AssertBotActions(
            new BotActions.DeleteMessage(
                ChatId: chatId,
                MessageId: messageId));
    }

    private static string CreateUserMention(
        User user)
    {
        var mention = user.Username ?? user.FirstName;
        return $"[{mention}](tg://user?{user.Id})";
    }
}