using System.Threading.Tasks;

namespace AdminBot.Common.Queries
{
    public class ChatSettingsQuery
    {
        public ChatSettingsQuery(long chatId)
        {
            ChatId = chatId;
        }
        
        public interface IHandler
        {
            Task<ChatSettings> HandleAsync(ChatSettingsQuery query);
        }
        
        public long ChatId { get; }
    }
}