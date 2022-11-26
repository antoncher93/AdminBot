using System.Text.RegularExpressions;
using AdminBot.Common;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AdminBot.Web.Tests;

public static class ObjectsGen
{
    public static User CreateUser(
        long userId,
        string? username = default)
    {
        return new User()
        {
            Id = userId,
            Username = username,
            FirstName = Gen.RandomString(),
        };
    }
    
    public static Update CreateMessageUpdate(
        Message message)
    {
        var update = new Update()
        {
            Message = message,
        };
        
        return update;
    }

    public static Update CreateMessageUpdate(
        int messageId,
        string text,
        long chatId,
        User? from,
        Message? replyToMessage = default)
    {
        var update = new Update()
        {
            Message = CreateMessage(
                messageId:  messageId,
                text: text,
                chatId: chatId,
                from: from,
                replyToMessage: replyToMessage),
        };
        
        return update;
    }
    
    public static Message CreateMessage(
        int messageId,
        long chatId,
        User? from,
        string? text = default,
        Message? replyToMessage = default,
        User[]? newChatMembers = default)
    {
        return new Message()
        {
            Chat = CreateChat(chatId),
            MessageId = Gen.RandomInt(),
            Date = DateTime.UtcNow,
            Text = text,
            Entities = GetEntities(text),
            ReplyToMessage = replyToMessage,
            From = from,
            NewChatMembers = newChatMembers,
        };
    }

    private static Chat CreateChat(long chatId)
    {
        return new Chat()
        {
            Id = chatId
        };
    }

    private static MessageEntity[]? GetEntities(string message)
    {
        if (message is null)
        {
            return null;
        }
        
        List<MessageEntity>? entities = null;
        
        var regex = new Regex(@"^\/\S*");
        var match = regex
            .Match(message);

        if (match.Success)
        {
            entities = entities ?? new List<MessageEntity>();
            
            entities.Add(new MessageEntity()
            {
                Type = MessageEntityType.BotCommand,
                Length = match.Length,
                Offset = match.Index,
            });
        }

        return entities?.ToArray();
    }

    public static Person CreateRandomPerson()
    {
        return new Person(
            id: Gen.RandomInt(),
            userId: Gen.RandomLong(),
            chatId: Gen.RandomLong(),
            username: Gen.RandomString(),
            warns: 0,
            createdAt: DateTime.Now,
            updatedAt: DateTime.Now);
    }

    public static ChatSettings CreateRandomChatSettings()
    {
        return new ChatSettings(
            telegramId: Gen.RandomLong(),
            agreement: Gen.RandomString(),
            warnsLimit: Gen.RandomInt(0, 10),
            banTtl: TimeSpan.FromHours(Gen.RandomInt(0, 999)),
            createdAt: DateTime.Now);
    }

    public static Ban CreateRandomBan(Person person)
    {
        return new Ban(
            person: person,
            createdAt: DateTime.Now,
            expireAt: DateTime.Now + TimeSpan.FromHours(Gen.RandomInt(5, 100)));
    }

    public static Update CreateCallbackQueryUpdate(
        User from,
        string callbackQueryData,
        int messageId,
        long chatId)
    {
        return new Update()
        {
            CallbackQuery = new CallbackQuery()
            {
                Data = callbackQueryData,
                From = from,
                Message = new Message()
                {
                    MessageId = messageId,
                    Chat = new Chat()
                    {
                        Id = chatId,
                    }
                }
            }
        };
    }
}