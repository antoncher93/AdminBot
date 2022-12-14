using AdminBot.UseCases.Providers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AdminBot.Web.Handlers.Internal;

internal class UpdateHandler : IUpdateHandler
{
    private readonly ICallbackQueryHandler _callbackQueryHandler;
    private readonly IMessageUpdateHandler _messageHandler;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateHandler(
        ICallbackQueryHandler callbackQueryHandler,
        IMessageUpdateHandler messageHandler,
        IDateTimeProvider dateTimeProvider)
    {
        _callbackQueryHandler = callbackQueryHandler;
        _messageHandler = messageHandler;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task HandleAsync(Update update)
    {
        if (update.Type == UpdateType.Message && update.Message != null)
        {
            var receivedAt = _dateTimeProvider.GetDateTimeNow();
            
            await _messageHandler
                .HandleAsync(
                    message: update.Message,
                    receivedAt: receivedAt);
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            if (update.CallbackQuery != null)
            {
                await _callbackQueryHandler.HandleAsync(
                    update: update);
            }
        }
    }
}