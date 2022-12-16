using System.Threading.Tasks;
using AdminBot.Common.Commands;
using AdminBot.Common.Messages;
using AdminBot.UseCases.Clients;
using AdminBot.UseCases.Repositories;

namespace AdminBot.UseCases.CommandHandlers
{
    public class WarnPersonCommandHandler : WarnPersonCommand.IHandler
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly IBotClient _botClient;

        public WarnPersonCommandHandler(
            IBotClient botClient,
            IPersonsRepository personsRepository)
        {
            _botClient = botClient;
            _personsRepository = personsRepository;
        }

        public async Task HandleAsync(
            WarnPersonCommand command)
        {
            var warns = await _personsRepository.IncrementWarnsAsync(
                person: command.Person,
                command.RequestTime);
            
            await _botClient
                .SendMessageAsync(
                    message: new WarnPersonMessage(
                        warnsLimit: command.WarnsLimit,
                        warns: warns,
                        userId: command.Person.UserId,
                        userName: command.Person.Username,
                        blameMessageId: command.MessageId),
                    chatId: command.Person.ChatId)
                .ConfigureAwait(false);
        }
    }
}