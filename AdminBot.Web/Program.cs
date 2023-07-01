using AdminBot.UseCases.Infrastructure.Providers;
using AdminBot.Web.Handlers;
using Serilog;
using Telegram.Bot.Hosting;

namespace AdminBot.Web;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo
            .File("adminbot.log")
            .CreateLogger();
        
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build()
            .Get<AppConfig>()!;

        BotHost.StartAsync(
            port: 10000,
            webhookHost: config.WebHookUrl,
            telegramBotToken: config.TelegramBotToken,
            botFacadeFactory: client => BotFacadeFactory.Create(
                sqlConnectionString: config.ProdSqlConnectionString,
                client: client,
                dateTimeProvider: new DateTimeProvider(),
                defaultBanTtl: config.DefaultBanTtl,
                botName: config.BotName,
                defaultWarnsLimit: config.DefaultWarnsLimit,
                descriptionFilePath: "./Description.md"));

        BotHost.WaitForShutdownAsync(CancellationToken.None);
    }
}