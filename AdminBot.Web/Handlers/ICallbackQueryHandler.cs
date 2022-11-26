using Telegram.Bot.Types;

namespace AdminBot.Web.Handlers;

public interface ICallbackQueryHandler
{
    Task HandleAsync(Update update);
}