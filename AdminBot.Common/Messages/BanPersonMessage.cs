using System;

namespace AdminBot.Common.Messages
{
    public class BanPersonMessage : IMessage
    {
        public BanPersonMessage(
            int blameMessageId,
            string userName,
            long userId,
            DateTime expireAt)
        {
            BlameMessageId = blameMessageId;
            ExpireAt = expireAt;
            UserId = userId;
            UserName = userName;
        }
        
        public int BlameMessageId { get; }
        public DateTime ExpireAt { get; }
        public string UserName { get; }
        public long UserId { get; }
    }
}