namespace AdminBot.Web.Tests.RepositoriesTests;

public class ChatSettingsRecord
{
    public long TelegramId { get; set; }
    public string? Agreement { get; set; }
    public int WarnsLimit { get; set; }
    public int BanTtlTicks { get; set; }
    public DateTime CreatedAt { get; set; }
}