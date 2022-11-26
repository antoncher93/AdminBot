using Telegram.Bot.Types;

namespace AdminBot.Web.Handlers;

public interface IMessageUpdateHandler
{
    Task HandleAsync(Message message, DateTime receivedAt);
}