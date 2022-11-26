namespace AdminBot.Web.Tests.RepositoriesTests;

public class BanRecord
{
    public BanRecord(
        int id,
        int personId,
        DateTime createdAt,
        DateTime expireAt)
    {
        PersonId = personId;
        Id = id;
        CreatedAt = createdAt;
        ExpireAt = expireAt;
    }

    public BanRecord()
    {
        
    }
    
    public int PersonId { get; set;}
    public int Id { get; set;}
    public DateTime CreatedAt { get; set;}
    public DateTime ExpireAt { get; set;}
}