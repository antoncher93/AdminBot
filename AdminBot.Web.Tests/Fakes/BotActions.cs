namespace AdminBot.Web.Tests.Fakes;

public static class BotActions
{
    public interface IBotAction
    {
    }
    
    public record TextMessage(
        string Text,
        long ChatId,
        int? ReplyToMessageId = default) : IBotAction;

    public record RestrictChatMember(
        long ChatId,
        long UserId,
        bool? CanSendMediaMessages = default,
        bool? CanSendMessages = default,
        bool? CanSendOtherMessages = default,
        bool? CanAddWebPagePreviews = default,
        bool? CanChangeInfo = default,
        bool? CanInviteUsers = default,
        bool? CanPinMessages = default,
        bool? CanSendPolls = default,
        DateTime? UntilDateTime = default) : IBotAction;

    public record DeleteMessage(
        long ChatId,
        int MessageId) : IBotAction;
}