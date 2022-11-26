using AdminBot.Common.CallbackQueries;
using AdminBot.Common.Messages;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace AdminBot.Web.Tests.BotLogicTests;

public class MessageHandlerTests
{
    [Fact]
    public async Task RestrictsNewChatMembers()
    {
        var sut = SutFactory.Create();

        var dateTime = DateTime.UtcNow;
        
        var chatId = Gen.RandomLong();

        var newUsers = Gen.ListOfValues(()
            => ObjectsGen.CreateUser(
                userId: Gen.RandomLong(),
                username: Gen.RandomString()))
            .ToArray();
        
        var newChatMemberMessage = ObjectsGen.CreateMessage(
            messageId: Gen.RandomInt(),
            chatId: chatId,
            from: null,
            newChatMembers: newUsers);

        var update = ObjectsGen.CreateMessageUpdate(newChatMemberMessage);
        
        var agreement = Gen.RandomString();

        await sut.SetChatSettingsAsync(
            chatId: chatId,
            agreement: agreement,
            dateTime: dateTime);

        await sut.HandleUpdateAsync(update, dateTime);

        var expectedRestrictedUserIds = newUsers
            .Select(user => user.Id)
            .ToList();

        sut.GetRestrictedUsers(chatId)
            .Should()
            .BeEquivalentTo(expectedRestrictedUserIds);

        var expectedMessages = newUsers
            .Select(user => new WelcomePersonMessage(
                userId: user.Id,
                userName: user.Username,
                agreement: agreement))
            .ToArray();
        
        sut.AssertChatMessages(
            chatId: chatId,
            messages: expectedMessages);
    }

    [Fact]
    public async Task RemovesRestrictionOnAcceptChatRules()
    {
        var sut = SutFactory.Create();

        var dateTime = DateTime.UtcNow;
        
        var chatId = Gen.RandomLong();

        var user = ObjectsGen.CreateUser(
            userId: Gen.RandomLong(),
            username: Gen.RandomString());
        
        var newChatMemberMessage = ObjectsGen.CreateMessage(
            messageId: Gen.RandomInt(),
            chatId: chatId,
            from: null,
            newChatMembers: new[]{ user });

        var update = ObjectsGen.CreateMessageUpdate(newChatMemberMessage);

        await sut.SetChatSettingsAsync(
            chatId: chatId,
            agreement: Gen.RandomString(),
            dateTime: DateTime.Today);

        await sut.HandleUpdateAsync(update, dateTime);
        
        var restrictedUsersBefore = sut.GetRestrictedUsers(chatId);

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
            update: callbackQueryUpdate, 
            dateTime: dateTime + TimeSpan.FromHours(1));

        var restrictedUsersAfter = sut.GetRestrictedUsers(chatId);

        restrictedUsersBefore
            .Should()
            .BeEquivalentTo(new List<long>() { user.Id });

        restrictedUsersAfter
            .Should()
            .BeEmpty();
    }

    [Fact]
    public async Task RemovesMessageOnAcceptClicked()
    {
        var sut = SutFactory.Create();

        var dateTime = DateTime.UtcNow;
        
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
            update: callbackQueryUpdate, 
            dateTime: dateTime + TimeSpan.FromHours(1));

        sut.GetDeletedMessages(chatId)
            .Should()
            .BeEquivalentTo(
                new[]
                {
                    messageId,
                });

    }
    
    
}