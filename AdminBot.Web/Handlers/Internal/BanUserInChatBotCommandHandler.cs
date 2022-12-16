using AdminBot.Common.Commands;
using AdminBot.Common.Queries;
using AdminBot.Web.BotCommands;

namespace AdminBot.Web.Handlers.Internal;

public class BanUserInChatBotCommandHandler : BanUserBotCommand.IHandler
{
    private readonly RegisterPersonQuery.IHandler _registerPersonQueryHandler;
    private readonly BanPersonCommand.IHandler _banPersonCommandHandler;
    private readonly IsUserAdminQuery.IHandler _isUserAdminCommandHandler;

    public BanUserInChatBotCommandHandler(
        RegisterPersonQuery.IHandler registerPersonQueryHandler,
        BanPersonCommand.IHandler banPersonCommandHandler,
        IsUserAdminQuery.IHandler isUserAdminCommandHandler)
    {
        _registerPersonQueryHandler = registerPersonQueryHandler;
        _banPersonCommandHandler = banPersonCommandHandler;
        _isUserAdminCommandHandler = isUserAdminCommandHandler;
    }

    public async Task HandleAsync(BanUserBotCommand command)
    {
        var isUserAdmin = await _isUserAdminCommandHandler
            .HandleAsync(
                query: new IsUserAdminQuery(
                    userId: command.SenderId,
                    chatId: command.ChatId));

        if (!isUserAdmin)
        {
            return;
        }
        
        var person = await _registerPersonQueryHandler
            .HandleAsync(
                query: new RegisterPersonQuery(
                    userId: command.UserId,
                    chatId: command.ChatId,
                    userName: command.Username,
                    dateTime: command.ExecutedAt));

        await _banPersonCommandHandler
            .HandleAsync(
                command: new BanPersonCommand(
                    person: person,
                    requestedAt: command.ExecutedAt,
                    messageId: command.BlameMessageId));
    }
}