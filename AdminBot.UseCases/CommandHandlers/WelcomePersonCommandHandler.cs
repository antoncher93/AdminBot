using System.Threading.Tasks;
using AdminBot.Common.Commands;
using AdminBot.Common.Messages;
using AdminBot.UseCases.Adapters;

namespace AdminBot.UseCases.CommandHandlers
{
    public class WelcomePersonCommandHandler : WelcomePersonCommand.IHandler
    {
        private readonly IBotClientAdapter _botClientAdapter;

        public WelcomePersonCommandHandler(
            IBotClientAdapter botClientAdapter)
        {
            _botClientAdapter = botClientAdapter;
        }

        public async Task HandleAsync(WelcomePersonCommand command)
        {
            await _botClientAdapter.RestrictUserInChatAsync(
                userId: command.Person.UserId,
                chatId: command.Person.ChatId);

            await _botClientAdapter.SendMessageAsync(
                message: new WelcomePersonMessage(
                    userName: command.Person.Username,
                    agreement: command.Agreement,
                    userId: command.Person.UserId),
                chatId: command.Person.ChatId);
        }
    }
}