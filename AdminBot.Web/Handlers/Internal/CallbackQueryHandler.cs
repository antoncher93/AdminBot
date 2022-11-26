using AdminBot.Common.CallbackQueries;
using AdminBot.UseCases.Clients;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace AdminBot.Web.Handlers.Internal;

public class CallbackQueryHandler : ICallbackQueryHandler
{
    private readonly IBotClient _botClient;

    public CallbackQueryHandler(IBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleAsync(Update update)
    {
        var data = update.CallbackQuery!.Data!;
        var callback = JsonConvert.DeserializeObject<CallbackQueryEnvelope>(data);

        if (callback != null)
        {
            await callback.Match(
                onAcceptChatRules: async acceptChatRulesQuery =>
                {
                    await this.OnAcceptChatRulesCallbackQuery(
                        acceptChatRulesQuery: acceptChatRulesQuery,
                        chatId: update!.CallbackQuery!.Message!.Chat.Id,
                        fromUserId: update.CallbackQuery.From.Id, 
                        queryMessageId: update.CallbackQuery.Message.MessageId);
                })!;
        }
    }

    private async Task OnAcceptChatRulesCallbackQuery(AcceptChatRulesCallbackQuery acceptChatRulesQuery,
        long chatId,
        long fromUserId,
        int queryMessageId)
    {
        if (fromUserId == acceptChatRulesQuery.UserId)
        {
            await _botClient.RemoveRestrictionAsync(
                userId: acceptChatRulesQuery.UserId,
                chatId: chatId);
            
            await _botClient.DeleteMessageAsync(
                chatId: chatId,
                messageId: queryMessageId);
        }
    }
}