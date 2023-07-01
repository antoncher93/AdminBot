using System.Threading.Tasks;
using AdminBot.Common.Queries;
using AdminBot.UseCases.Adapters;

namespace AdminBot.UseCases.QueryHandlers
{
    public class IsUserAdminQueryHandler : IsUserAdminQuery.IHandler
    {
        private readonly IBotClientAdapter _botClientAdapter;

        public IsUserAdminQueryHandler(IBotClientAdapter botClientAdapter)
        {
            _botClientAdapter = botClientAdapter;
        }

        public Task<bool> HandleAsync(IsUserAdminQuery query)
        {
            return _botClientAdapter.IsUserAdminAsync(
                userId: query.UserId,
                chatId: query.ChatId);
        }
    }
}