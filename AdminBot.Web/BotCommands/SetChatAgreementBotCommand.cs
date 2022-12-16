namespace AdminBot.Web.BotCommands;

public class SetChatAgreementBotCommand
{
    public SetChatAgreementBotCommand(
        int messageId,
        string agreementText,
        long chatId,
        long senderId,
        DateTime requestedAt)
    {
        AgreementText = agreementText;
        ChatId = chatId;
        SenderId = senderId;
        RequestedAt = requestedAt;
        MessageId = messageId;
    }
    
    public interface IHandler
    {
        Task HandleAsync(SetChatAgreementBotCommand command);
    }
    public string AgreementText { get; }
    public long ChatId { get; }
    public long SenderId { get; }
    public DateTime RequestedAt { get; }
    public int MessageId { get; }
}