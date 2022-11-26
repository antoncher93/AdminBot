using System.Threading.Tasks;
using AdminBot.Common.Queries;
using AdminBot.UseCases.Clients;

namespace AdminBot.UseCases.QueryHandlers
{
    public class IsUserAdminQueryHandler : IsUserAdminQuery.IHandler
    {
        private readonly IBotClient _botClient;

        public IsUserAdminQueryHandler(IBotClient botClient)
        {
            _botClient = botClient;
        }

        public Task<bool> HandleAsync(IsUserAdminQuery query)
        {
            return _botClient.IsUserAdminAsync(
                userId: query.UserId,
                chatId: query.ChatId);
        }
    }
}