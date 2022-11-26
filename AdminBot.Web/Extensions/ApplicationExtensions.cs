using AdminBot.UseCases.Clients;

namespace AdminBot.Web.Extensions;

public static class ApplicationExtensions
{
    private static void TelelegramWebHook(
        this WebApplication app,
        string webhookUrl)
    {
        if (app is null)
        {
            throw new ArgumentException();
        }
        
        app.Services
            .GetService<IBotClient>()
            .SetWebHook(webhookUrl);
    }
}