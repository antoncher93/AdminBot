using System.Threading.Tasks;
using AdminBot.Common.Commands;
using AdminBot.UseCases.Clients;

namespace AdminBot.UseCases.CommandHandlers
{
    public class RemoveRestrictionCommandHandler : RemoveRestrictionCommand.IHandler
    {
        private readonly IBotClient _client;

        public RemoveRestrictionCommandHandler(
            IBotClient client)
        {
            _client = client;
        }

        public Task HandleAsync(
            RemoveRestrictionCommand command)
        {
            return _client.RemoveRestrictionAsync(
                userId: command.UserId,
                chatId: command.ChatId);
        }
    }
}