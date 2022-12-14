using AdminBot.Common.Commands;
using AdminBot.Common.Queries;
using AdminBot.Web.BotCommands;

namespace AdminBot.Web.Handlers.Internal;

public class SetChatAgreementBotCommandHandler : SetChatAgreementBotCommand.IHandler
{
    private readonly SaveChatSettingsCommand.IHandler _saveChatSettingsCommandHandler;
    private readonly ChatSettingsQuery.IHandler _chatSettingsQueryHandler;
    private readonly IsUserAdminQuery.IHandler _isUserAdminQueryHandler;
    private readonly int _defaultWarnsLimit;
    private readonly TimeSpan _defaultBanTtl;
    
    public SetChatAgreementBotCommandHandler(
        SaveChatSettingsCommand.IHandler saveChatSettingsCommandHandler,
        IsUserAdminQuery.IHandler isUserAdminQueryHandler,
        ChatSettingsQuery.IHandler chatSettingsQueryHandler,
        int defaultWarnsLimit,
        TimeSpan defaultBanTtl)
    {
        _saveChatSettingsCommandHandler = saveChatSettingsCommandHandler;
        _isUserAdminQueryHandler = isUserAdminQueryHandler;
        _defaultWarnsLimit = defaultWarnsLimit;
        _defaultBanTtl = defaultBanTtl;
        _chatSettingsQueryHandler = chatSettingsQueryHandler;
    }

    public async Task HandleAsync(
        SetChatAgreementBotCommand command)
    {
        var isUserAdmin = await _isUserAdminQueryHandler
            .HandleAsync(
                query: new IsUserAdminQuery(
                    userId: command.SenderId,
                    chatId: command.ChatId));

        if (!isUserAdmin)
        {
            return;
        }

        var chatSettings = await _chatSettingsQueryHandler
            .HandleAsync(
                query: new ChatSettingsQuery(
                    chatId: command.ChatId));

        if (chatSettings is null)
        {
            await _saveChatSettingsCommandHandler
                .HandleAsync(
                    command: new SaveChatSettingsCommand(
                        telegramId: command.ChatId,
                        agreement: command.AgreementText,
                        warnsLimit: _defaultWarnsLimit,
                        banTtl: _defaultBanTtl,
                        executedAt: command.RequestedAt));
        }
        else
        {
            await _saveChatSettingsCommandHandler
                .HandleAsync(
                    command: new SaveChatSettingsCommand(
                        telegramId: command.ChatId,
                        agreement: command.AgreementText,
                        warnsLimit: chatSettings.WarnsLimit,
                        banTtl: chatSettings.BanTtl,
                        executedAt: command.RequestedAt));
        }
    }
}