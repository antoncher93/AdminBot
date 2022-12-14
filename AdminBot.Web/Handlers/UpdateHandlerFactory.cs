using AdminBot.UseCases.Clients;
using AdminBot.UseCases.CommandHandlers;
using AdminBot.UseCases.Infrastructure;
using AdminBot.UseCases.Infrastructure.Internal;
using AdminBot.UseCases.Infrastructure.Repositories;
using AdminBot.UseCases.Providers;
using AdminBot.UseCases.QueryHandlers;
using AdminBot.Web.Handlers.Internal;

namespace AdminBot.Web.Handlers;

public static class UpdateHandlerFactory
{
    public static IUpdateHandler Create(string sqlConnectionString,
        IBotClient botClient,
        IDateTimeProvider dateTimeProvider,
        TimeSpan defaultBanTtl,
        string botName,
        int defaultWarnsLimit,
        string descriptionFilePath)
    {
        var dbConnectionFactory = new SqlConnectionFactory(
            connectionString: sqlConnectionString);

        var personsRepository = PersonsRepositoryFactory.Create(
            sqlConnectionString: sqlConnectionString);
        
        var chatSettingsRepository = ChatSettingsRepositoryFactory.Create(
            sqlConnectionString: sqlConnectionString);

        var registerPersonQueryHandler = new RegisterPersonQueryHandler(
            persons: personsRepository);

        var chatSettingsQueryHandler = new ChatSettingsQueryHandler(
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
            chatSettingsQueryHandler: chatSettingsQueryHandler,
            isUserAdminHandler: isUserAdminQueryHandler,
            defaultWarnsLimit: defaultWarnsLimit);

        var banUserBotCommandHandler = new BanUserInChatBotCommandHandler(
            registerPersonQueryHandler: registerPersonQueryHandler,
            isUserAdminCommandHandler: isUserAdminQueryHandler,
            banPersonCommandHandler: banPersonCommandHandler);
        
        var saveChatAgreementCommandHandler = new SaveChatAgreementCommandHandler(
            chatSettings: chatSettingsRepository,
            botClient: botClient);

        var setChatSettingsBotCommandHandler = new SetChatAgreementBotCommandHandler(
            saveChatSettingsCommandHandler: saveChatAgreementCommandHandler,
            defaultBanTtl: defaultBanTtl, isUserAdminQueryHandler: isUserAdminQueryHandler,
            chatSettingsQueryHandler: chatSettingsQueryHandler,
            defaultWarnsLimit: defaultWarnsLimit);

        var botCommandUpdateHandler = new BotCommandMessageHandler(
            warnUserBotCommandHandler: warnUserBotCommandHandler,
            banUserBotCommandHandler: banUserBotCommandHandler,
            botName: botName,
            setChatAgreementBotCommandHandler: setChatSettingsBotCommandHandler,
            showDescriptionCommandHandler: new ShowDescriptionCommandHandler(
                botClient: botClient,
                descriptionFilePath: descriptionFilePath));

        var welcomePersonCommandHandler = new WelcomePersonCommandHandler(
            botClient: botClient);

        var messageUpdateHandler = new MessageUpdateHandler(
            botCommandMessageHandler: botCommandUpdateHandler,
            registerPersonCommandHandler: registerPersonQueryHandler,
            welcomePersonCommandHandler: welcomePersonCommandHandler,
            chatSettingsQueryHandler: chatSettingsQueryHandler);

        return new UpdateHandler(
            dateTimeProvider: dateTimeProvider,
            messageHandler: messageUpdateHandler,
            callbackQueryHandler: new CallbackQueryHandler(
                botClient: botClient));
    }
}