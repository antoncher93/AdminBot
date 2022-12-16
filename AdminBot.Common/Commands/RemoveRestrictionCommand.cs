using System.Threading.Tasks;

namespace AdminBot.Common.Commands;

public class RemoveRestrictionCommand
{
    public RemoveRestrictionCommand(
        long userId,
        long chatId)
    {
        UserId = userId;
        ChatId = chatId;
    }
    
    public interface IHandler
    {
        Task HandleAsync(RemoveRestrictionCommand command);
    }
    public long UserId { get; }
    public long ChatId { get; }

}