using AdminBot.Common.Commands;
using AdminBot.Common.Queries;
using AdminBot.Web.BotCommands;

namespace AdminBot.Web.Handlers.Internal;

public class WarnUserBotCommandHandler : WarnUserBotCommand.IHandler
{
    private readonly RegisterPersonQuery.IHandler _registerPersonQueryHandler;
    private readonly WarnPersonCommand.IHandler _warnPersonCommandHandler;
    private readonly BanPersonCommand.IHandler _banPersonCommandHandler;
    private readonly ChatSettingsQuery.IHandler _chatSettingsQueryHandler;
    private readonly int _defaultWarnsLimit;

    public WarnUserBotCommandHandler(
        RegisterPersonQuery.IHandler registerPersonQueryHandler,
        WarnPersonCommand.IHandler warnPersonCommandHandler,
        BanPersonCommand.IHandler banPersonCommandHandler,
        ChatSettingsQuery.IHandler chatSettingsQueryHandler,
        int defaultWarnsLimit)
    {
        _registerPersonQueryHandler = registerPersonQueryHandler;
        _warnPersonCommandHandler = warnPersonCommandHandler;
        this._banPersonCommandHandler = banPersonCommandHandler;
        _chatSettingsQueryHandler = chatSettingsQueryHandler;
        _defaultWarnsLimit = defaultWarnsLimit;
    }

    public async Task HandleAsync(WarnUserBotCommand command)
    {
        var person = await _registerPersonQueryHandler
            .HandleAsync(
                query: new RegisterPersonQuery(
                    userId: command.UserId,
                    chatId: command.ChatId,
                    userName: command.Username,
                    dateTime: command.ExecutedAt))
            .ConfigureAwait(false);
        
        var chatSettings = await _chatSettingsQueryHandler
            .HandleAsync(
                query: new ChatSettingsQuery(
                    chatId: command.ChatId))
            .ConfigureAwait(false);

        var warnsLimit = chatSettings?.WarnsLimit ?? _defaultWarnsLimit;

        if (person.Warns >= warnsLimit)
        {
            await _banPersonCommandHandler
                .HandleAsync(
                    command: new BanPersonCommand(
                        person: person,
                        messageId: command.BlameMessageId,
                        requestedAt: command.ExecutedAt))
                .ConfigureAwait(false);
        }
        else
        {
            await _warnPersonCommandHandler.HandleAsync(
                command: new WarnPersonCommand(
                    person: person,
                    warnsLimit: warnsLimit,
                    messageId: command.BlameMessageId,
                    requestTime: command.ExecutedAt))
                .ConfigureAwait(false);
        }
    }
}