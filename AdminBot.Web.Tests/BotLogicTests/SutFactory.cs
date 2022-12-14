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
        var fakeBotClient = new FakeBotClient();

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build()
            .Get<AppConfig>();

        var fakeDateTimeProvider = new FakeDateTimeProvider();

        var updateHandler = UpdateHandlerFactory.Create(
            sqlConnectionString: config.TestSqlConnectionString,
            botClient: fakeBotClient,
            defaultBanTtl: config.DefaultBanTtl,
            botName: botName ?? config.BotName,
            defaultWarnsLimit: config.DefaultWarnsLimit,
            descriptionFilePath: config.DescriptionFilePath,
            dateTimeProvider: fakeDateTimeProvider);

        var personsRepository = PersonsRepositoryFactory.Create(config.TestSqlConnectionString);

        var chatSettings = ChatSettingsRepositoryFactory.Create(config.TestSqlConnectionString);
        
        var chatAgreementQueryHandler = new ChatSettingsQueryHandler(
            chatSettings: chatSettings);

        var saveChatSettingsCommandHandler = new SaveChatAgreementCommandHandler(
            chatSettings: chatSettings,
            botClient: fakeBotClient);

        return new Sut(
            updateHandler: updateHandler,
            fakeBotClient: fakeBotClient,
            chatAgreementQueryHandler: chatAgreementQueryHandler,
            personsRepository: personsRepository,
            saveChatSettingsCommandHandler: saveChatSettingsCommandHandler,
            dateTimeProvider: fakeDateTimeProvider,
            defaultBanTtl: config.DefaultBanTtl,
            defaultWarnsLimit: config.DefaultWarnsLimit);
    }
}