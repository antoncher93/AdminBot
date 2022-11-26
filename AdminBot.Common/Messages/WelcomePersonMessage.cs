namespace AdminBot.Common.Messages
{
    public class WelcomePersonMessage : IMessage
    {
        public WelcomePersonMessage(
            long userId,
            string userName,
            string agreement)
        {
            UserId = userId;
            Agreement = agreement;
            UserName = userName;
        }
        public long UserId { get; }
        public string Agreement { get; }
        public string UserName { get; }
    }
}