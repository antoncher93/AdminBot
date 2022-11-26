using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AdminBot.Web.Extensions;

public static class MessageExtensions
{
    public static T Match<T>(
        this Message message,
        Func<Message, T>? onTextMessage = default,
        Func<Message, T>? onChatMembersAdded = default,
        Func<Message, T>? onDefault = default)
    {
        if (message.Type == MessageType.Text)
        {
            if (onTextMessage is null)
            {
                throw new ArgumentException();
            }

            return onTextMessage(message);
        }
        else if(message.Type == MessageType.ChatMembersAdded)
        {
            if (onChatMembersAdded is null)
            {
                throw new ArgumentException();
            }
            return onChatMembersAdded(message);
        }
        else
        {
            if (onDefault is null)
            {
                throw new ArgumentException();
            }

            return onDefault(message);
        }
    }
}