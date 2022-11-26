namespace AdminBot.Common.Messages
{
    public class ChatRulesHasBeenChangedMessage : IMessage
    {
        public ChatRulesHasBeenChangedMessage(string agreement)
        {
            Agreement = agreement;
        }

        public string Agreement { get; }
    }
}