using AdminBot.Common.Commands;
using AdminBot.Common.Queries;
using AdminBot.Web.BotCommands;
using Telegram.Bot.Types;

namespace AdminBot.Web.Handlers.Internal;

internal class BotCommandMessageHandler : IBotCommandMessageHandler
{
    private readonly string _botName;
    private readonly WarnUserBotCommand.IHandler _warnUserBotCommandHandler;
    private readonly BanUserBotCommand.IHandler _banUserBotCommandHandler;
    private readonly SetChatAgreementBotCommand.IHandler _setChatAgreementBotCommandHandler;
    private readonly StartBotCommandCommand.IHandler _showDescriptionCommandHandler;

    public BotCommandMessageHandler(
        WarnUserBotCommand.IHandler warnUserBotCommandHandler,
        BanUserBotCommand.IHandler banUserBotCommandHandler,
        SetChatAgreementBotCommand.IHandler setChatAgreementBotCommandHandler,
        StartBotCommandCommand.IHandler showDescriptionCommandHandler,
        string botName)
    {
        _warnUserBotCommandHandler = warnUserBotCommandHandler;
        _banUserBotCommandHandler = banUserBotCommandHandler;
        _setChatAgreementBotCommandHandler = setChatAgreementBotCommandHandler;
        _showDescriptionCommandHandler = showDescriptionCommandHandler;
        _botName = botName;
    }

    public async Task HandleAsync(
        string command,
        Message message,
        DateTime receivedAt)
    {
        if (this.IsEqualsCommand(
                input: command,
                knownCommand: "/start"))
        {
            await _showDescriptionCommandHandler
                .HandleAsync(
                    commandCommand: new StartBotCommandCommand(
                        chatId: message.Chat.Id));
        }
        else if (this.IsEqualsCommand(
                     input: command,
                     knownCommand: "/warn"))
        {
            if (message.ReplyToMessage != null)
            {
                await _warnUserBotCommandHandler
                    .HandleAsync(
                        command: new WarnUserBotCommand(
                            userId: message.ReplyToMessage.From.Id,
                            senderId: message.From.Id,
                            chatId: message.Chat.Id,
                            username: message.ReplyToMessage.From.Username,
                            firstName: message.ReplyToMessage.From.FirstName,
                            blameMessageId: message.ReplyToMessage.MessageId,
                            executedAt: receivedAt));    
            }
        }
        else if (this.IsEqualsCommand(
                     input: command,
                     knownCommand: "/ban"))
        {
            if (message.ReplyToMessage != null)
            {
                await _banUserBotCommandHandler
                    .HandleAsync(
                        command: new BanUserBotCommand(
                            userId: message.ReplyToMessage.From.Id,
                            chatId: message.Chat.Id,
                            username: message.ReplyToMessage.From.Username,
                            firstName: message.ReplyToMessage.From.FirstName,
                            blameMessageId: message.ReplyToMessage.MessageId,
                            senderId: message.From.Id,
                            executedAt: receivedAt));
            }
        }
        else if (this.IsEqualsCommand(
                      input: command,
                      knownCommand: "/setAgreement"))
        {
            if (message.ReplyToMessage != null)
            {
                await _setChatAgreementBotCommandHandler
                    .HandleAsync(
                        command: new SetChatAgreementBotCommand(
                            chatId: message.Chat.Id,
                            senderId: message.From.Id,
                            agreementText: message.ReplyToMessage.Text,
                            requestedAt: receivedAt));
            }
        }
    }

    private bool IsEqualsCommand(
        string input,
        string knownCommand)
    {
        return input == knownCommand
               || input == $"{knownCommand}@{_botName}";
    }
}