using System;

namespace AdminBot.UseCases.Infrastructure.DataModels
{
    public class ChatSettings
    {
        public ChatSettings(
            long telegramId,
            string agreement,
            int warnsLimit,
            int banTtlTicks,
            DateTime createdAt)
        {
            TelegramId = telegramId;
            Agreement = agreement;
            WarnsLimit = warnsLimit;
            BanTtlTicks = banTtlTicks;
            CreatedAt = createdAt;
        }

        public ChatSettings()
        {
            
        }
        
        public int Id { get; set;}
        
        public long TelegramId { get; set;}
        
        public string Agreement { get; set;}
        
        public int WarnsLimit { get; set;}
        
        public int BanTtlTicks { get; set;}
        
        public DateTime CreatedAt { get; set; }
    }
}