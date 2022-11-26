using AdminBot.UseCases.Clients;
using AdminBot.UseCases.Infrastructure.Clients;
using AdminBot.Web.Handlers;
using AdminBot.Web.Middlewares;
using Serilog;

namespace AdminBot.Web;

public static class ApplicationRoot
{
    public static void ConfigureServices(IServiceCollection services,
        string sqlConnectionString,
        IBotClient botClient,
        TimeSpan defaultBanTtl,
        int defaultWarnsLimit,
        string descriptionFilePath)
    {
        services.AddControllers()
            .AddNewtonsoftJson();
        
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen();
        
        services.AddUpdateHandler(
            sqlConnectionString: sqlConnectionString,
            botClient: botClient,
            defaultBanTtl: defaultBanTtl,
            defaultWarnsLimit: defaultWarnsLimit,
            descriptionFilePath: descriptionFilePath);
    }
    
    public static WebApplication BuildApp(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo
            .File("adminbot.log")
            .CreateLogger();
        
        var builder = WebApplication.CreateBuilder(args);
        
        var appConfig = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build()
            .Get<AppConfig>();

        var botClient = BotClientFactory.Create(appConfig.TelegramBotToken);
        
        botClient.SetWebHook(appConfig.WebHookUrl);
        
        Log.Logger.Information($"Telegram Webhook start on {appConfig.WebHookUrl}");
        
        ConfigureServices(builder.Services,
            sqlConnectionString: appConfig.ProdSqlConnectionString,
            botClient: botClient,
            defaultBanTtl: appConfig.DefaultBanTtl,
            defaultWarnsLimit: appConfig.DefaultWarnsLimit,
            descriptionFilePath: appConfig.DescriptionFilePath);
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.UseExceptionHandling();

        return app;
    }
}