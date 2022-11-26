using AdminBot.UseCases.Clients;

namespace AdminBot.Web.Handlers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUpdateHandler(
        this IServiceCollection services,
        string sqlConnectionString,
        IBotClient botClient,
        TimeSpan defaultBanTtl,
        int defaultWarnsLimit,
        string descriptionFilePath)
    {
        services.AddSingleton<IUpdateHandler>(UpdateHandlerFactory.Create(
            sqlConnectionString: sqlConnectionString,
            botClient: botClient,
            defaultBanTtl: defaultBanTtl,
            defaultWarnsLimit: defaultWarnsLimit,
            descriprionFilePath: descriptionFilePath));

        return services;
    }
}