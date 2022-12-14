using System.Threading.Tasks;

namespace AdminBot.Common.Commands;

public class StartBotCommandCommand
{
    public StartBotCommandCommand(long chatId)
    {
        ChatId = chatId;
    }
    
    public interface IHandler
    {
        Task HandleAsync(StartBotCommandCommand commandCommand);
    }
    public long ChatId { get; }
}