using System;

namespace AdminBot.Common
{
    public class Ban
    {
        public Ban(
            Person person,
            DateTime expireAt,
            DateTime createdAt)
        {
            Person = person;
            ExpireAt = expireAt;
            CreatedAt = createdAt;
        }
        
        public Person Person { get; }
        public DateTime ExpireAt { get; }
        public DateTime CreatedAt { get; }
    }
}