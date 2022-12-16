using AdminBot.Common;
using AdminBot.Common.Commands;
using AdminBot.Common.Messages;
using AdminBot.Common.Queries;
using AdminBot.UseCases.Providers;
using AdminBot.UseCases.Repositories;
using AdminBot.Web.Handlers;
using AdminBot.Web.Tests.Fakes;
using FluentAssertions;
using FluentAssertions.Extensions;
using Telegram.Bot.Types;

namespace AdminBot.Web.Tests.BotLogicTests;

public class Sut
{
    private readonly IUpdateHandler _updateHandler;
    private readonly FakeBotClient _fakeBotClient;
    private readonly ChatSettingsQuery.IHandler _chatAgreementQueryHandler;
    private readonly IPersonsRepository _personsRepository;
    private readonly SaveChatSettingsCommand.IHandler _saveChatSettingsCommandHandler;
    private readonly IDateTimeProvider _dateTimeProvider;

    public Sut(
        IUpdateHandler updateHandler,
        FakeBotClient fakeBotClient,
        ChatSettingsQuery.IHandler chatAgreementQueryHandler,
        IPersonsRepository personsRepository,
        SaveChatSettingsCommand.IHandler saveChatSettingsCommandHandler,
        IDateTimeProvider dateTimeProvider,
        int defaultWarnsLimit, TimeSpan defaultBanTtl)
    {
        _updateHandler = updateHandler;
        _fakeBotClient = fakeBotClient;
        _chatAgreementQueryHandler = chatAgreementQueryHandler;
        DefaultWarnsLimit = defaultWarnsLimit;
        DefaultBanTtl = defaultBanTtl;
        _dateTimeProvider = dateTimeProvider;
        _saveChatSettingsCommandHandler = saveChatSettingsCommandHandler;
        _personsRepository = personsRepository;
    }
    
    public int DefaultWarnsLimit { get; }
    public TimeSpan DefaultBanTtl { get; }

    public Task HandleUpdateAsync(
        Update update)
    {
        return _updateHandler
            .HandleAsync(
                update: update);
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
        int warnsLimit,
        TimeSpan banTtl)
    {
        return _saveChatSettingsCommandHandler
            .HandleAsync(
                command: new SaveChatSettingsCommand(
                    telegramId: chatId,
                    agreement: agreement,
                    warnsLimit: warnsLimit,
                    banTtl: banTtl,
                    executedAt: _dateTimeProvider.GetDateTimeNow()));
    }

    public DateTime GetProvidedDateTime()
    {
        return _dateTimeProvider.GetDateTimeNow();
    }

    public async Task SetupPersonAsync(
        long userId,
        long chatId,
        string? userName,
        string firstName,
        int warns,
        DateTime createdAt,
        DateTime updatedAt)
    {
        var person = await _personsRepository.AddPersonAsync(
            userId: userId,
            username: userName,
            chatId: chatId, createdAt: createdAt);

        for (int i = 0; i < warns; i++)
        {
            await _personsRepository
                .IncrementWarnsAsync(
                    person: person,
                    dateTime: updatedAt);
        }
    }
}