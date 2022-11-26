using System.Threading.Tasks;
using AdminBot.Common.Commands;
using AdminBot.Common.Messages;
using AdminBot.UseCases.Clients;

namespace AdminBot.UseCases.CommandHandlers
{
    public class WelcomePersonCommandHandler : WelcomePersonCommand.IHandler
    {
        private readonly IBotClient _botClient;

        public WelcomePersonCommandHandler(
            IBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task HandleAsync(WelcomePersonCommand command)
        {
            await _botClient.RestrictUserInChatAsync(
                userId: command.Person.UserId,
                chatId: command.Person.ChatId);

            await _botClient.SendMessageAsync(
                message: new WelcomePersonMessage(
                    userName: command.Person.Username,
                    agreement: command.Agreement,
                    userId: command.Person.UserId),
                chatId: command.Person.ChatId);
        }
    }
}