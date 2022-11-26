using System.Threading.Tasks;

namespace AdminBot.Common.Commands;

public class ShowDescriptionCommand
{
    public ShowDescriptionCommand(long chatId)
    {
        ChatId = chatId;
    }
    
    public interface IHandler
    {
        Task HandleAsync(ShowDescriptionCommand command);
    }
    public long ChatId { get; }
}