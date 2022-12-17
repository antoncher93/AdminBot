using AdminBot.Common.CallbackQueries;
using AdminBot.Common.Commands;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace AdminBot.Web.Handlers.Internal;

public class CallbackQueryHandler : ICallbackQueryHandler
{
    private readonly DeleteMessageCommand.IHandler _deleteMessageCommandHandler;
    private readonly RemoveRestrictionCommand.IHandler _removeRestrictionCommandHandler;

    public CallbackQueryHandler(
        DeleteMessageCommand.IHandler deleteMessageCommandHandler,
        RemoveRestrictionCommand.IHandler removeRestrictionCommandHandler)
    {
        _deleteMessageCommandHandler = deleteMessageCommandHandler;
        _removeRestrictionCommandHandler = removeRestrictionCommandHandler;
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
            await _removeRestrictionCommandHandler
                .HandleAsync(
                    command: new RemoveRestrictionCommand(
                        userId: acceptChatRulesQuery.UserId,
                        chatId: chatId));
            
            await _deleteMessageCommandHandler.HandleAsync(
                command: new DeleteMessageCommand(
                    chatId: chatId,
                    messageId: queryMessageId));
        }
    }
}