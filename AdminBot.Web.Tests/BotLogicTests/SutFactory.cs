using AdminBot.UseCases.CommandHandlers;
using AdminBot.UseCases.Infrastructure;
using AdminBot.UseCases.QueryHandlers;
using AdminBot.Web.Handlers;
using AdminBot.Web.Tests.BotCommandsTests;
using AdminBot.Web.Tests.Fakes;
using Microsoft.Extensions.Configuration;

namespace AdminBot.Web.Tests.BotLogicTests;

public static class SutFactory
{
    public static Sut Create()
    {
        var fakeBotClient = new FakeBotClient();

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build()
            .Get<AppConfig>();

        var updateHandler = UpdateHandlerFactory.Create(
            sqlConnectionString: config.TestSqlConnectionString,
            botClient: fakeBotClient,
            defaultBanTtl: config.DefaultBanTtl,
            defaultWarnsLimit: config.DefaultWarnsLimit,
            descriprionFilePath: config.DescriptionFilePath);

        var personsRepository = PersonsRepositoryFactory.Create(config.TestSqlConnectionString);

        var chatSettings = ChatSettingsRepositoryFactory.Create(config.TestSqlConnectionString,
            defaultBanTtl: config.DefaultBanTtl,
            defaultWarnsLimit: config.DefaultWarnsLimit);
        
        var chatAgreementQueryHandler = new ChatAgreementQueryHandler(
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
            defaultBanTtl: config.DefaultBanTtl,
            defaultWarnsLimit: config.DefaultWarnsLimit);
    }
}