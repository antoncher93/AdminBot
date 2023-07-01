using System.Threading.Tasks;
using AdminBot.Common.Commands;
using AdminBot.UseCases.Adapters;

namespace AdminBot.UseCases.CommandHandlers
{
    public class DeleteMessageCommandHandler : DeleteMessageCommand.IHandler
    {
        private readonly IBotClientAdapter _botClientAdapter;

        public DeleteMessageCommandHandler(
            IBotClientAdapter botClientAdapter)
        {
            _botClientAdapter = botClientAdapter;
        }

        public Task HandleAsync(
            DeleteMessageCommand command)
        {
            return _botClientAdapter.DeleteMessageAsync(
                chatId: command.ChatId,
                messageId: command.MessageId);
        }
    }
}