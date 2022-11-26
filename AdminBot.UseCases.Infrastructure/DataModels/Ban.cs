using System;

namespace AdminBot.UseCases.Infrastructure.DataModels
{
    public class Ban
    {
        public Ban()
        {
            
        }

        public Ban(int id, int personId, DateTime createdAt, DateTime expireAt)
        {
            Id = id;
            PersonId = personId;
            CreatedAt = createdAt;
            ExpireAt = expireAt;
        }
        public int Id { get; set; }
        public int PersonId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}