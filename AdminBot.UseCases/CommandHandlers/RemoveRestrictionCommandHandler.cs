using System.Threading.Tasks;
using AdminBot.Common.Commands;
using AdminBot.UseCases.Adapters;

namespace AdminBot.UseCases.CommandHandlers
{
    public class RemoveRestrictionCommandHandler : RemoveRestrictionCommand.IHandler
    {
        private readonly IBotClientAdapter _clientAdapter;

        public RemoveRestrictionCommandHandler(
            IBotClientAdapter clientAdapter)
        {
            _clientAdapter = clientAdapter;
        }

        public Task HandleAsync(
            RemoveRestrictionCommand command)
        {
            return _clientAdapter.RemoveRestrictionAsync(
                userId: command.UserId,
                chatId: command.ChatId);
        }
    }
}