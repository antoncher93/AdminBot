using System.Threading.Tasks;
using AdminBot.Common.Commands;
using AdminBot.Common.Messages;
using AdminBot.UseCases.Adapters;
using AdminBot.UseCases.Repositories;

namespace AdminBot.UseCases.CommandHandlers
{
    public class WarnPersonCommandHandler : WarnPersonCommand.IHandler
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly IBotClientAdapter _botClientAdapter;

        public WarnPersonCommandHandler(
            IBotClientAdapter botClientAdapter,
            IPersonsRepository personsRepository)
        {
            _botClientAdapter = botClientAdapter;
            _personsRepository = personsRepository;
        }

        public async Task HandleAsync(
            WarnPersonCommand command)
        {
            var warns = await _personsRepository.IncrementWarnsAsync(
                person: command.Person,
                command.RequestTime);
            
            await _botClientAdapter
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