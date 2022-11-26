namespace AdminBot.Common.Messages;

public class DescriptionMessage : IMessage
{
    public DescriptionMessage(string description)
    {
        Description = description;
    }

    public string Description { get; }
}