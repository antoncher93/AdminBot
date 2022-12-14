using System;
using System.Threading.Tasks;

namespace AdminBot.Common.Commands
{
    public class SaveChatSettingsCommand
    {
        public SaveChatSettingsCommand(
            long telegramId,
            string agreement,
            int warnsLimit,
            TimeSpan banTtl,
            DateTime executedAt)
        {
            TelegramId = telegramId;
            Agreement = agreement;
            ExecutedAt = executedAt;
            WarnsLimit = warnsLimit;
            BanTtl = banTtl;
        }
        
        public interface IHandler
        {
            Task HandleAsync(SaveChatSettingsCommand command);
        }
        
        public long TelegramId { get; }
        
        public string Agreement { get; }
        
        public DateTime ExecutedAt { get; }
        
        public int WarnsLimit { get; }
        
        public TimeSpan BanTtl { get; }
    }
}