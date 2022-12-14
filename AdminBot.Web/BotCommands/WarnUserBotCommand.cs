namespace AdminBot.Web.BotCommands;

public class WarnUserBotCommand
{
    public WarnUserBotCommand(
        long userId,
        long senderId,
        long chatId,
        string username,
        string firstName,
        int blameMessageId,
        DateTime executedAt)
    {
        UserId = userId;
        ChatId = chatId;
        BlameMessageId = blameMessageId;
        ExecutedAt = executedAt;
        FirstName = firstName;
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
    public string FirstName { get; }
}