using AdminBot.Common;
using AdminBot.Common.Queries;
using AdminBot.UseCases.Infrastructure.Extensions;
using AdminBot.UseCases.Providers;
using AdminBot.UseCases.Repositories;
using AdminBot.Web.Tests.Fakes;
using FluentAssertions;
using Telegram.Bot.Hosting;
using Telegram.Bot.Types;

namespace AdminBot.Web.Tests.BotLogicTests;

public class Sut
{
    private readonly IBotFacade _botFacade;
    private readonly FakeTelegramBotClient _fakeTelegramBotClient;
    private readonly ChatSettingsQuery.IHandler _chatAgreementQueryHandler;
    private readonly IPersonsRepository _personsRepository;
    private readonly IChatSettingsRepository _chatSettingsRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public Sut(
        IBotFacade botFacade,
        FakeTelegramBotClient fakeTelegramBotClient,
        ChatSettingsQuery.IHandler chatAgreementQueryHandler,
        IPersonsRepository personsRepository,
        IChatSettingsRepository chatSettingsRepository,
        IDateTimeProvider dateTimeProvider,
        int defaultWarnsLimit,
        TimeSpan defaultBanTtl)
    {
        _botFacade = botFacade;
        _fakeTelegramBotClient = fakeTelegramBotClient;
        _chatAgreementQueryHandler = chatAgreementQueryHandler;
        DefaultWarnsLimit = defaultWarnsLimit;
        DefaultBanTtl = defaultBanTtl;
        _chatSettingsRepository = chatSettingsRepository;
        _dateTimeProvider = dateTimeProvider;
        _personsRepository = personsRepository;
    }
    
    public int DefaultWarnsLimit { get; }
    public TimeSpan DefaultBanTtl { get; }

    public Task HandleUpdateAsync(
        Update update)
    {
        return _botFacade
            .OnUpdateAsync(
                update: update);
    }

    public void AssertBotActions(
        params BotActions.IBotAction[] boActions)
    {
        _fakeTelegramBotClient.GetBotActions()
            .Should()
            .BeEquivalentTo(
                expectation: boActions,
                config: options => options
                    .WithStrictOrdering()
                    .RespectingRuntimeTypes()
                    .ComparingByMembers<BotActions.RestrictChatMember>()
                    .ComparingByMembers<BotActions.DeleteMessage>()
                    .ComparingByMembers<BotActions.TextMessage>());
    }

    public Task<ChatSettings> FindChatAgreementAsync(long chatId)
    {
        return _chatAgreementQueryHandler
            .HandleAsync(
                query: new ChatSettingsQuery(
                    chatId: chatId));
    }

    public User CreateChatMember(long chatId)
    {
        var user = ObjectsGen.CreateUser();
        
        _fakeTelegramBotClient.CreateChatMember(
            user: user,
            chatId: chatId);

        return user;
    }

    public User CreateChatAdmin(long chatId)
    {
        var user = ObjectsGen.CreateUser();
        
        _fakeTelegramBotClient.CreateChatAdmin(
            user: user,
            chatId: chatId);

        return user;
    }

    public Task SetChatSettingsAsync(
        long chatId,
        string agreement,
        int warnsLimit,
        TimeSpan banTtl)
    {
        return _chatSettingsRepository.SaveAgreementAsync(
            telegramId: chatId,
            agreement: agreement,
            banTtl: banTtl,
            warnsLimit: warnsLimit,
            dateTime: _dateTimeProvider.GetUtcNow());
    }

    public DateTime GetUtcNow()
    {
        return _dateTimeProvider.GetUtcNow();
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