using System.Threading.Tasks;

namespace AdminBot.Common.Commands;

public class DeleteMessageCommand
{
    public DeleteMessageCommand(long chatId, int messageId)
    {
        ChatId = chatId;
        MessageId = messageId;
    }
    
    public interface IHandler
    {
        Task HandleAsync(
            DeleteMessageCommand command);
    }
    public long ChatId { get; }
    
    public int MessageId { get; }
}