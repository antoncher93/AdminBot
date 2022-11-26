using AdminBot.Common;
using AdminBot.Common.Commands;
using AdminBot.Common.Messages;
using AdminBot.Common.Queries;
using AdminBot.UseCases.Repositories;
using AdminBot.Web.Handlers;
using AdminBot.Web.Tests.Fakes;
using FluentAssertions;
using FluentAssertions.Extensions;
using Telegram.Bot.Types;

namespace AdminBot.Web.Tests.BotCommandsTests;

public class Sut
{
    private readonly IUpdateHandler _updateHandler;
    private readonly FakeBotClient _fakeBotClient;
    private readonly ChatSettingsQuery.IHandler _chatAgreementQueryHandler;
    private readonly IPersonsRepository _personsRepository;
    private readonly SaveChatSettingsCommand.IHandler _saveChatSettingsCommandHandler;

    public Sut(
        IUpdateHandler updateHandler,
        FakeBotClient fakeBotClient,
        ChatSettingsQuery.IHandler chatAgreementQueryHandler,
        IPersonsRepository personsRepository,
        SaveChatSettingsCommand.IHandler saveChatSettingsCommandHandler,
        int defaultWarnsLimit,
        TimeSpan defaultBanTtl)
    {
        _updateHandler = updateHandler;
        _fakeBotClient = fakeBotClient;
        _chatAgreementQueryHandler = chatAgreementQueryHandler;
        DefaultWarnsLimit = defaultWarnsLimit;
        DefaultBanTtl = defaultBanTtl;
        _saveChatSettingsCommandHandler = saveChatSettingsCommandHandler;
        _personsRepository = personsRepository;
    }
    
    public int DefaultWarnsLimit { get; }
    public TimeSpan DefaultBanTtl { get; }

    public Task HandleUpdateAsync(
        Update update,
        DateTime dateTime)
    {
        return _updateHandler.HandleAsync(update, dateTime);
    }
    
    public void AssertChatMessages(
        long chatId,
        params IMessage[] messages)
    {
        var actual = _fakeBotClient.GetBotMessages(chatId);
        actual.Should()
            .BeEquivalentTo(
                messages,
                config: options =>
                {
                    options
                        .RespectingRuntimeTypes()
                        .ComparingByMembers<WarnPersonMessage>()
                        .ComparingByMembers<BanPersonMessage>()
                        .ComparingByMembers<ChatRulesHasBeenChangedMessage>()
                        .Using<DateTime>(ctx
                                => ctx.Subject
                                    .Should()
                                    .BeCloseTo(ctx.Expectation, 1.Seconds()))
                            .WhenTypeIs<DateTime>();
                    return options;
                });
    }

    public IMessage[] GetBotMessages(
        long chatId)
    {
        return _fakeBotClient.GetBotMessages(chatId);
    }

    public Task<Person> FindPerson(
        long userId,
        long chatId)
    {
        return _personsRepository.GetPersonAsync(
            userId: userId,
            chatId: chatId);
    }

    public Task<ChatSettings> FindChatAgreementAsync(long chatId)
    {
        return _chatAgreementQueryHandler
            .HandleAsync(
                query: new ChatSettingsQuery(
                    chatId: chatId));
    }

    public List<long> GetRestrictedUsers(long chatId)
    {
        return _fakeBotClient
            .GetRestrictedUsers(chatId)
            .ToList();
    }

    public void AddChatAdmin(User admin, long chatId)
    {
        _fakeBotClient.SetupChatAdmin(admin.Id, chatId);
    }

    public int?[] GetDeletedMessages(long chatId)
    {
        return _fakeBotClient.GetDeletedMessages(chatId);
    }

    public Task SetChatSettingsAsync(
        long chatId,
        string agreement,
        DateTime dateTime)
    {
        return _saveChatSettingsCommandHandler
            .HandleAsync(
                command: new SaveChatSettingsCommand(
                    telegramId: chatId,
                    agreement: agreement,
                    executedAt: dateTime));
    }
}