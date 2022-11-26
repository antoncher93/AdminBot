using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AdminBot.Web.Handlers.Internal;

internal class UpdateHandler : IUpdateHandler
{
    private readonly ICallbackQueryHandler _callbackQueryHandler;
    private readonly IMessageUpdateHandler _messageHandler;

    public UpdateHandler(
        ICallbackQueryHandler callbackQueryHandler,
        IMessageUpdateHandler messageHandler)
    {
        _callbackQueryHandler = callbackQueryHandler;
        _messageHandler = messageHandler;
    }

    public async Task HandleAsync(Update update, DateTime receivedAt)
    {
        if (update.Type == UpdateType.Message && update.Message != null)
        {
            await _messageHandler.HandleAsync(update.Message, receivedAt);
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            if (update.CallbackQuery != null)
            {
                await _callbackQueryHandler.HandleAsync(
                    update: update);
            }
        }
        else if (update.Type == UpdateType.ChatMember)
        {
            
        }
    }
}