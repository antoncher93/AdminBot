using System;

namespace AdminBot.Common.Messages
{
    public class BanPersonMessage : IMessage
    {
        public BanPersonMessage(
            Person person,
            int blameMessageId,
            DateTime expireAt)
        {
            BlameMessageId = blameMessageId;
            ExpireAt = expireAt;
            Person = person;
        }
        
        public Person Person { get; }
        public int BlameMessageId { get; }
        public DateTime ExpireAt { get; }
    }
}