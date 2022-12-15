namespace AdminBot.Web;

public class AppConfig
{
    public string ProdSqlConnectionString { get; set; } = string.Empty;
    
    public string TestSqlConnectionString { get; set; } = string.Empty;
    public string TelegramBotToken { get; set; } = string.Empty;
    public string TelegramBotTokenDev { get; set; } = string.Empty;
    public string WebHookUrl { get; set; } = string.Empty;
    public TimeSpan DefaultBanTtl { get; set; } = TimeSpan.FromHours(3);
    public int DefaultWarnsLimit { get; set; } = 2;
    public string DescriptionFilePath { get; set; } = "./Description";
    public string BotName { get; set; }
}