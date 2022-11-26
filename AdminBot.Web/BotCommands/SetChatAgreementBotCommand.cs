namespace AdminBot.Web.BotCommands;

public class SetChatAgreementBotCommand
{
    public SetChatAgreementBotCommand(
        string agreementText,
        long chatId,
        long fromUserId,
        DateTime requestedAt)
    {
        AgreementText = agreementText;
        ChatId = chatId;
        FromUserId = fromUserId;
        RequestedAt = requestedAt;
    }
    
    public interface IHandler
    {
        Task HandleAsync(SetChatAgreementBotCommand command);
    }
    public string AgreementText { get; }
    public long ChatId { get; }
    public long FromUserId { get; }
    public DateTime RequestedAt { get; }
}