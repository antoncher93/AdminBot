using Telegram.Bot.Types;

namespace AdminBot.Web.Handlers;

public interface IBotCommandMessageHandler
{
    Task HandleAsync(string command,
        Message message,
        DateTime receivedAt);
}