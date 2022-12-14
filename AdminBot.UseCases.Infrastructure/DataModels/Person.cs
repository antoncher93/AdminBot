using System;

namespace AdminBot.UseCases.Infrastructure.DataModels
{
    public class Person
    {
        public Person(
            int id,
            long userId,
            string username,
            long chatId,
            int warns,
            DateTime createdAt,
            DateTime updatedAt)
        {
            Id = id;
            UserId = userId;
            Username = username;
            ChatId = chatId;
            Warns = warns;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public Person()
        {
            
        }
        
        public int Id { get; set; }
        
        public long UserId { get; set; }
        
        public string Username { get; set; }
        
        public long ChatId { get; set; }
        
        public int Warns { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public string FirstName { get; set; }
    }
}