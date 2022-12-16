using System;

namespace AdminBot.Common
{
    public class Person
    {
        public Person(
            int id,
            long userId,
            string username,
            long chatId,
            DateTime createdAt,
            int warns,
            DateTime updatedAt)
        {
            Id = id;
            UserId = userId;
            Username = username;
            ChatId = chatId;
            CreatedAt = createdAt;
            Warns = warns;
            UpdatedAt = updatedAt;
        }
        
        public int Id { get; }
        
        public long UserId { get; }
        
        public string Username { get; }
        
        public long ChatId { get; }
        
        public DateTime CreatedAt { get; }
        
        public DateTime UpdatedAt { get; }
        
        public int Warns { get; }
    }
}