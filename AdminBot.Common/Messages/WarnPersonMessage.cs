namespace AdminBot.Common.Messages
{
    public class WarnPersonMessage : IMessage
    {
        public WarnPersonMessage(
            Person person,
            int? blameMessageId,
            int warnsLimit)
        {
            BlameMessageId = blameMessageId;
            WarnsLimit = warnsLimit;
            Person = person;
        }
        
        public Person Person  { get; }
        public int? BlameMessageId { get; }
        public int WarnsLimit { get; }
    }
}