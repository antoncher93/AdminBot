using AdminBot.UseCases.CommandHandlers;
using AdminBot.UseCases.Infrastructure;
using AdminBot.UseCases.Infrastructure.Clients;
using AdminBot.UseCases.Infrastructure.Internal;
using AdminBot.UseCases.Infrastructure.Repositories;
using AdminBot.UseCases.Providers;
using AdminBot.UseCases.QueryHandlers;
using AdminBot.Web.Handlers.Internal;
using Telegram.Bot;
using Telegram.Bot.Hosting;

namespace AdminBot.Web.Handlers;

public static class BotFacadeFactory
{
    public static IBotFacade Create(
        string sqlConnectionString,
        ITelegramBotClient client,
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
        
        var botClient = new BotClientAdapterAdapter(
            client: client, 
            messageFormatter: new MessageFormatter(client));

        var isUserAdminQueryHandler = new IsUserAdminQueryHandler(
            botClientAdapter: botClient);

        var warnPersonCommandHandler = new WarnPersonCommandHandler(
            botClientAdapter: botClient,
            personsRepository: personsRepository);

        var banRepository = new BanRepository(
            dbConnectionFactory: dbConnectionFactory);

        var banPersonCommandHandler = new BanPersonCommandHandler(
            personsRepository: personsRepository,
            banRepository: banRepository,
            chatSettingsRepository: chatSettingsRepository,
            botClientAdapter: botClient,
            defaultBanTtl: defaultBanTtl);
        
        var deleteMessageCommandHandler = new DeleteMessageCommandHandler(
            botClientAdapter: botClient);

        var warnUserBotCommandHandler = new WarnUserBotCommandHandler(
            registerPersonQueryHandler: registerPersonQueryHandler,
            warnPersonCommandHandler: warnPersonCommandHandler,
            banPersonCommandHandler: banPersonCommandHandler,
            chatSettingsQueryHandler: chatSettingsQueryHandler,
            isUserAdminHandler: isUserAdminQueryHandler,
            deleteMessageCommandHandler: deleteMessageCommandHandler,
            defaultWarnsLimit: defaultWarnsLimit);

        var banUserBotCommandHandler = new BanUserInChatBotCommandHandler(
            registerPersonQueryHandler: registerPersonQueryHandler,
            isUserAdminCommandHandler: isUserAdminQueryHandler,
            banPersonCommandHandler: banPersonCommandHandler,
            deleteMessageCommandHandler: deleteMessageCommandHandler);
        
        var saveChatAgreementCommandHandler = new SaveChatAgreementCommandHandler(
            chatSettings: chatSettingsRepository,
            botClientAdapter: botClient);

        var removeRestrictionCommandHandler = new RemoveRestrictionCommandHandler(
            clientAdapter: botClient);

       var setChatSettingsBotCommandHandler = new SetChatAgreementBotCommandHandler(
            saveChatSettingsCommandHandler: saveChatAgreementCommandHandler,
            defaultBanTtl: defaultBanTtl, isUserAdminQueryHandler: isUserAdminQueryHandler,
            deleteMessageCommandHandler: deleteMessageCommandHandler,
            chatSettingsQueryHandler: chatSettingsQueryHandler,
            defaultWarnsLimit: defaultWarnsLimit);

        var botCommandUpdateHandler = new BotCommandMessageHandler(
            warnUserBotCommandHandler: warnUserBotCommandHandler,
            banUserBotCommandHandler: banUserBotCommandHandler,
            botName: botName,
            setChatAgreementBotCommandHandler: setChatSettingsBotCommandHandler,
            showDescriptionCommandHandler: new ShowDescriptionCommandHandler(
                botClientAdapter: botClient,
                descriptionFilePath: descriptionFilePath));

        var welcomePersonCommandHandler = new WelcomePersonCommandHandler(
            botClientAdapter: botClient);

        var messageUpdateHandler = new MessageUpdateHandler(
            botCommandMessageHandler: botCommandUpdateHandler,
            registerPersonCommandHandler: registerPersonQueryHandler,
            welcomePersonCommandHandler: welcomePersonCommandHandler,
            chatSettingsQueryHandler: chatSettingsQueryHandler);

        return new UpdateHandler(
            dateTimeProvider: dateTimeProvider,
            messageHandler: messageUpdateHandler,
            callbackQueryHandler: new CallbackQueryHandler(
                removeRestrictionCommandHandler: removeRestrictionCommandHandler,
                deleteMessageCommandHandler: deleteMessageCommandHandler));
    }
}