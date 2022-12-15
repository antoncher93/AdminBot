using AdminBot.UseCases.Clients;
using AdminBot.UseCases.Providers;

namespace AdminBot.Web.Handlers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUpdateHandler(
        this IServiceCollection services,
        string sqlConnectionString,
        IBotClient botClient,
        IDateTimeProvider dateTimeProvider,
        TimeSpan defaultBanTtl,
        string botName,
        int defaultWarnsLimit,
        string descriptionFilePath)
    {
        services.AddSingleton(UpdateHandlerFactory.Create(
            sqlConnectionString: sqlConnectionString,
            botClient: botClient,
            defaultBanTtl: defaultBanTtl,
            botName: botName,
            defaultWarnsLimit: defaultWarnsLimit,
            descriptionFilePath: descriptionFilePath,
            dateTimeProvider: dateTimeProvider));

        return services;
    }
}