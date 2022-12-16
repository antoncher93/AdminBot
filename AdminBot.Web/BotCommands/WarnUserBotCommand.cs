namespace AdminBot.Web.BotCommands;

public class WarnUserBotCommand
{
    public WarnUserBotCommand(
        int messageId,
        long userId,
        long senderId,
        long chatId,
        string username,
        int blameMessageId,
        DateTime executedAt)
    {
        UserId = userId;
        ChatId = chatId;
        BlameMessageId = blameMessageId;
        ExecutedAt = executedAt;
        MessageId = messageId;
        SenderId = senderId;
        Username = username;
    }
    
    public interface IHandler
    {
        Task HandleAsync(WarnUserBotCommand command);
    }
    
    public long UserId { get; }
    public long SenderId { get; }
    public long ChatId { get; }
    public string Username { get; }
    public int BlameMessageId { get; }
    public DateTime ExecutedAt { get; }
    public int MessageId { get; }
}