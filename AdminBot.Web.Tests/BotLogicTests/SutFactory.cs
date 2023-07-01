using AdminBot.UseCases.CommandHandlers;
using AdminBot.UseCases.Infrastructure;
using AdminBot.UseCases.QueryHandlers;
using AdminBot.Web.Handlers;
using AdminBot.Web.Tests.Fakes;
using Microsoft.Extensions.Configuration;

namespace AdminBot.Web.Tests.BotLogicTests;

public static class SutFactory
{
    public static Sut Create(
        string? botName = null)
    {
        var fakeBotClient = new FakeTelegramBotClient();

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build()
            .Get<AppConfig>();

        var fakeDateTimeProvider = new FakeDateTimeProvider();

        var botFacade = BotFacadeFactory.Create(
            sqlConnectionString: config.TestSqlConnectionString,
            client: fakeBotClient,
            defaultBanTtl: config.DefaultBanTtl,
            botName: botName ?? config.BotName,
            defaultWarnsLimit: config.DefaultWarnsLimit,
            descriptionFilePath: config.DescriptionFilePath,
            dateTimeProvider: fakeDateTimeProvider);

        var personsRepository = PersonsRepositoryFactory.Create(config.TestSqlConnectionString);

        var chatSettingsRepository = ChatSettingsRepositoryFactory.Create(config.TestSqlConnectionString);
        
        var chatAgreementQueryHandler = new ChatSettingsQueryHandler(
            chatSettings: chatSettingsRepository);

        return new Sut(
            botFacade: botFacade,
            fakeTelegramBotClient: fakeBotClient,
            chatAgreementQueryHandler: chatAgreementQueryHandler,
            personsRepository: personsRepository,
            chatSettingsRepository: chatSettingsRepository,
            dateTimeProvider: fakeDateTimeProvider,
            defaultBanTtl: config.DefaultBanTtl,
            defaultWarnsLimit: config.DefaultWarnsLimit);
    }
}