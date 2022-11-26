using AdminBot.Common.Commands;
using AdminBot.Common.Queries;
using AdminBot.Web.BotCommands;
using Telegram.Bot.Types;

namespace AdminBot.Web.Handlers.Internal;

internal class BotCommandMessageHandler : IBotCommandMessageHandler
{
    private readonly WarnUserBotCommand.IHandler _warnUserBotCommandHandler;
    private readonly BanUserBotCommand.IHandler _banUserBotCommandHandler;
    private readonly SetChatAgreementBotCommand.IHandler _setChatAgreementBotCommandHandler;
    private readonly IsUserAdminQuery.IHandler _isUserAdminQueryHandler;
    private readonly ShowDescriptionCommand.IHandler _showDescriptionCommandHandler;

    public BotCommandMessageHandler(
        WarnUserBotCommand.IHandler warnUserBotCommandHandler,
        BanUserBotCommand.IHandler banUserBotCommandHandler,
        SetChatAgreementBotCommand.IHandler setChatAgreementBotCommandHandler,
        IsUserAdminQuery.IHandler isUserAdminQueryHandler,
        ShowDescriptionCommand.IHandler showDescriptionCommandHandler)
    {
        _warnUserBotCommandHandler = warnUserBotCommandHandler;
        _banUserBotCommandHandler = banUserBotCommandHandler;
        _setChatAgreementBotCommandHandler = setChatAgreementBotCommandHandler;
        _isUserAdminQueryHandler = isUserAdminQueryHandler;
        _showDescriptionCommandHandler = showDescriptionCommandHandler;
    }

    public async Task HandleAsync(string command,
        Message message,
        DateTime receivedAt)
    {
        switch (command)
        {
            case "/start":
            {
                await _showDescriptionCommandHandler
                    .HandleAsync(
                        command: new ShowDescriptionCommand(
                            chatId: message.Chat.Id));
                break;
            }
            case "/warn":
            {
                var isUserAdmin = await _isUserAdminQueryHandler
                    .HandleAsync(
                        query: new IsUserAdminQuery(
                            userId: message.From.Id,
                            chatId: message.Chat.Id));
                
                if (!isUserAdmin)
                {
                    return;
                }
                
                if (message.ReplyToMessage is null)
                {
                    break;
                }

                await _warnUserBotCommandHandler.HandleAsync(
                    command: new WarnUserBotCommand(
                        userId: message.ReplyToMessage.From.Id,
                        chatId: message.Chat.Id,
                        username: message.ReplyToMessage.From.Username,
                        blameMessageId: message.ReplyToMessage.MessageId,
                        executedAt: receivedAt));
                
                break;
            }

            case "/ban":
            {
                var isUserAdmin = await _isUserAdminQueryHandler
                    .HandleAsync(
                        query: new IsUserAdminQuery(
                            userId: message.From.Id,
                            chatId: message.Chat.Id));
                
                if (!isUserAdmin)
                {
                    return;
                }
                
                if (message.ReplyToMessage?.From is null)
                {
                    break;
                }

                await _banUserBotCommandHandler.HandleAsync(
                    command: new BanUserBotCommand(
                        userId: message.ReplyToMessage.From.Id,
                        chatId: message.Chat.Id,
                        username: message.ReplyToMessage.From.Username,
                        blameMessageId: message.ReplyToMessage.MessageId,
                        executedAt: receivedAt));
                
                break;
            }
            case "/setAgreement":
            {
                var isUserAdmin = await _isUserAdminQueryHandler
                    .HandleAsync(
                        query: new IsUserAdminQuery(
                            userId: message.From.Id,
                            chatId: message.Chat.Id));
                
                if (!isUserAdmin)
                {
                    return;
                }
                
                if (message.ReplyToMessage is null)
                {
                    break;
                }

                await _setChatAgreementBotCommandHandler
                    .HandleAsync(
                        command: new SetChatAgreementBotCommand(
                            chatId: message.Chat.Id,
                            fromUserId: message.From.Id,
                            agreementText: message.ReplyToMessage.Text,
                            requestedAt: receivedAt));
                
                break;
            }
        }
    }
}