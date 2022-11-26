using AdminBot.Common.Commands;
using AdminBot.Common.Queries;
using AdminBot.Web.BotCommands;

namespace AdminBot.Web.Handlers.Internal;

public class BanUserInChatBotCommandHandler : BanUserBotCommand.IHandler
{
    private readonly RegisterPersonQuery.IHandler _registerPersonQueryHandler;
    private readonly BanPersonCommand.IHandler _banPersonCommandHandler;

    public BanUserInChatBotCommandHandler(
        RegisterPersonQuery.IHandler registerPersonQueryHandler,
        BanPersonCommand.IHandler banPersonCommandHandler)
    {
        _registerPersonQueryHandler = registerPersonQueryHandler;
        _banPersonCommandHandler = banPersonCommandHandler;
    }

    public async Task HandleAsync(BanUserBotCommand command)
    {
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