using AdminBot.Common.Commands;
using AdminBot.Common.Messages;
using AdminBot.UseCases.Clients;
using AdminBot.Web.BotCommands;

namespace AdminBot.Web.Handlers.Internal;

public class SetChatAgreementBotCommandHandler : SetChatAgreementBotCommand.IHandler
{
    private readonly SaveChatSettingsCommand.IHandler _saveChatSettingsCommandHandler;
    
    public SetChatAgreementBotCommandHandler(
        SaveChatSettingsCommand.IHandler saveChatSettingsCommandHandler)
    {
        _saveChatSettingsCommandHandler = saveChatSettingsCommandHandler;
    }

    public async Task HandleAsync(SetChatAgreementBotCommand command)
    {
        await _saveChatSettingsCommandHandler
            .HandleAsync(
                command: new SaveChatSettingsCommand(
                    telegramId: command.ChatId,
                    agreement: command.AgreementText,
                    executedAt: command.RequestedAt));
    }
}