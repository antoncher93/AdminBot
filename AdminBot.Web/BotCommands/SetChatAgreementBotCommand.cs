namespace AdminBot.Web.BotCommands;

public class SetChatAgreementBotCommand
{
    public SetChatAgreementBotCommand(
        string agreementText,
        long chatId,
        long senderId,
        DateTime requestedAt)
    {
        AgreementText = agreementText;
        ChatId = chatId;
        SenderId = senderId;
        RequestedAt = requestedAt;
    }
    
    public interface IHandler
    {
        Task HandleAsync(SetChatAgreementBotCommand command);
    }
    public string AgreementText { get; }
    public long ChatId { get; }
    public long SenderId { get; }
    public DateTime RequestedAt { get; }
}