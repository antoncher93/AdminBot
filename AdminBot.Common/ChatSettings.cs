using System;

namespace AdminBot.Common
{
    public class ChatSettings
    {
        public ChatSettings(
            long telegramId,
            string agreement,
            int warnsLimit,
            TimeSpan banTtl,
            DateTime createdAt)
        {
            TelegramId = telegramId;
            Agreement = agreement;
            WarnsLimit = warnsLimit;
            BanTtl = banTtl;
            CreatedAt = createdAt;
        }
        
        public long TelegramId { get; }
        
        public string Agreement { get; }
        
        public int WarnsLimit { get; }
        public DateTime CreatedAt { get; }
        
        public TimeSpan BanTtl { get; }
    }
}