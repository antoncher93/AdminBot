namespace AdminBot.Web.Tests.RepositoriesTests;

public class PersonRecord
{
    public int Id { get; set;}
    
    public long UserId { get; set;}
        
    public string? Username { get; set;}
    
    public string FirstName { get; set; }
        
    public long ChatId { get; set;}
        
    public int Warns { get; set; }
        
    public bool IsBanned { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}