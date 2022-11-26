using System.Threading.Tasks;

namespace AdminBot.Common.Queries
{
    public class IsUserAdminQuery
    {
        public IsUserAdminQuery(
            long userId,
            long chatId)
        {
            UserId = userId;
            ChatId = chatId;
        }
        
        public interface IHandler
        {
            Task<bool> HandleAsync(
                IsUserAdminQuery query);
        }
        
        public long UserId { get; }
        public long ChatId { get; }
    }
}