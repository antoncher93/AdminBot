using Telegram.Bot.Types;

namespace AdminBot.Web.Handlers;

public interface IUpdateHandler
{
    Task HandleAsync(Update update, DateTime receivedAt);
}