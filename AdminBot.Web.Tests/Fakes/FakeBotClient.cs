using AdminBot.Common.Messages;
using AdminBot.UseCases.Clients;

namespace AdminBot.Web.Tests.Fakes;

public class FakeBotClient : IBotClient
{
    private readonly Dictionary<long, List<IMessage>> _messages = new();
    private readonly Dictionary<long, List<long>> _restrictedPersons = new();
    private readonly Dictionary<long, List<long>> _chatAdmins = new();
    private readonly Dictionary<long, List<int?>> _deletedMessages = new();

    public Task RestrictUserInChatAsync(long userId,
        long chatId, DateTime? untilDateTime)
    {
        if (_restrictedPersons.ContainsKey(chatId))
        {
            _restrictedPersons[chatId].Add(userId);
        }
        else
        {
            _restrictedPersons.Add(chatId, new List<long>()
            {
                userId,
            });
        }
        return Task.CompletedTask;
    }

    public Task RemoveRestrictionAsync(long userId, long chatId)
    {
        if (_restrictedPersons.ContainsKey(chatId))
        {
            if (_restrictedPersons[chatId].Contains(userId))
            {
                _restrictedPersons[chatId].Remove(userId);
            }
        }
        
        return Task.CompletedTask;
    }

    public Task SendMessageAsync(
        IMessage message,
        long chatId)
    {
        if (_messages.ContainsKey(chatId))
        {
            _messages[chatId].Add(message);
        }
        else
        {
            _messages.Add(
                key: chatId,
                new List<IMessage>()
                {
                    message
                });
        }
        
        return Task.CompletedTask;
    }

    public Task<bool> IsUserAdminAsync(long userId, long chatId)
    {
        return Task.FromResult(
            result: _chatAdmins.ContainsKey(chatId) && _chatAdmins[chatId].Contains(userId));
    }

    public void SetWebHook(string webhookUrl)
    {
        
    }

    public Task DeleteMessageAsync(long chatId, int messageId)
    {
        if (_deletedMessages.ContainsKey(chatId))
        {
            _deletedMessages[chatId].Add(messageId);
        }
        else
        {
            _deletedMessages.Add(chatId, new List<int?>(){ messageId});
        }

        return Task.CompletedTask;
    }

    public int?[] GetDeletedMessages(long chatId)
    {
        return _deletedMessages[chatId].ToArray();
    }

    public IMessage[] GetBotMessages(long chatId)
    {
        return _messages[chatId].ToArray();
    }

    public List<long> GetRestrictedUsers(long chatId)
    {
        return _restrictedPersons[chatId];
    }

    public void SetupChatAdmin(
        long userId,
        long chatId)
    {
        if (_chatAdmins.ContainsKey(chatId))
        {
            _chatAdmins[chatId].Add(userId);
        }
        else
        {
            _chatAdmins.Add(chatId, new List<long>(){userId});
        }
    }
}