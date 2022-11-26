using AdminBot.UseCases.Clients;
using AdminBot.UseCases.CommandHandlers;
using AdminBot.UseCases.Infrastructure;
using AdminBot.UseCases.Infrastructure.Internal;
using AdminBot.UseCases.Infrastructure.Repositories;
using AdminBot.UseCases.QueryHandlers;
using AdminBot.Web.Handlers.Internal;

namespace AdminBot.Web.Handlers;

public static class UpdateHandlerFactory
{
    public static IUpdateHandler Create(
        string sqlConnectionString,
        IBotClient botClient,
        TimeSpan defaultBanTtl,
        int defaultWarnsLimit,
        string descriprionFilePath)
    {
        var dbConnectionFactory = new SqlConnectionFactory(
            connectionString: sqlConnectionString);

        var personsRepository = PersonsRepositoryFactory.Create(
            sqlConnectionString: sqlConnectionString);
        
        var chatSettingsRepository = ChatSettingsRepositoryFactory.Create(
            sqlConnectionString: sqlConnectionString,
            defaultBanTtl: defaultBanTtl,
            defaultWarnsLimit: defaultWarnsLimit);

        var registerPersonQueryHandler = new RegisterPersonQueryHandler(
            persons: personsRepository);

        var chatAgreementQueryHandler = new ChatAgreementQueryHandler(
            chatSettings: chatSettingsRepository);

        var isUserAdminQueryHandler = new IsUserAdminQueryHandler(
            botClient: botClient);

        var warnPersonCommandHandler = new WarnPersonCommandHandler(
            botClient: botClient,
            personsRepository: personsRepository);

        var banRepository = new BanRepository(
            dbConnectionFactory: dbConnectionFactory);

        var banPersonCommandHandler = new BanPersonCommandHandler(
            personsRepository: personsRepository,
            banRepository: banRepository,
            chatSettingsRepository: chatSettingsRepository,
            botClient: botClient,
            defaultBanTtl: defaultBanTtl);

        var warnUserBotCommandHandler = new WarnUserBotCommandHandler(
            registerPersonQueryHandler: registerPersonQueryHandler,
            warnPersonCommandHandler: warnPersonCommandHandler,
            banPersonCommandHandler: banPersonCommandHandler,
            chatSettingsQueryHandler: chatAgreementQueryHandler,
            defaultWarnsLimit: defaultWarnsLimit);

        var banUserBotCommandHandler = new BanUserInChatBotCommandHandler(
            registerPersonQueryHandler: registerPersonQueryHandler,
            banPersonCommandHandler: banPersonCommandHandler);
        
        var saveChatAgreementCommandHandler = new SaveChatAgreementCommandHandler(
            chatSettings: chatSettingsRepository,
            botClient: botClient);

        var setChatSettingsBotCommandHandler = new SetChatAgreementBotCommandHandler(
            saveChatSettingsCommandHandler: saveChatAgreementCommandHandler);

        var botCommandUpdateHandler = new BotCommandMessageHandler(
            isUserAdminQueryHandler: isUserAdminQueryHandler,
            warnUserBotCommandHandler: warnUserBotCommandHandler,
            banUserBotCommandHandler: banUserBotCommandHandler,
            setChatAgreementBotCommandHandler: setChatSettingsBotCommandHandler,
            showDescriptionCommandHandler: new ShowDescriptionCommandHandler(
                botClient: botClient,
                descriptionFilePath: descriprionFilePath));

        var welcomePersonCommandHandler = new WelcomePersonCommandHandler(
            botClient: botClient);

        var messageUpdateHandler = new MessageUpdateHandler(
            botCommandMessageHandler: botCommandUpdateHandler,
            registerPersonCommandHandler: registerPersonQueryHandler,
            welcomePersonCommandHandler: welcomePersonCommandHandler,
            chatSettingsQueryHandler: chatAgreementQueryHandler);

        return new UpdateHandler(
            messageHandler: messageUpdateHandler,
            callbackQueryHandler: new CallbackQueryHandler(
                botClient: botClient));
    }
}