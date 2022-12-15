namespace AdminBot.Common.Messages
{
    public class WarnPersonMessage : IMessage
    {
        public WarnPersonMessage(
            int? blameMessageId,
            string userName,
            long userId,
            int warns,
            int warnsLimit)
        {
            BlameMessageId = blameMessageId;
            WarnsLimit = warnsLimit;
            Warns = warns;
            UserId = userId;
            UserName = userName;
        }
        
        public int? BlameMessageId { get; }
        public int WarnsLimit { get; }
        public string UserName { get; }
        public long UserId { get; }
        public int Warns { get; }
    }
}