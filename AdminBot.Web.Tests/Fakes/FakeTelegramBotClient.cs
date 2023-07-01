using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace AdminBot.Web.Tests.Fakes;

public class FakeTelegramBotClient : ITelegramBotClient
{
    private readonly List<BotActions.IBotAction> _botActions = new();
    private readonly Dictionary<long, List<ChatMember>> _chats = new();
    
    public bool LocalBotServer { get; }
    
    public long? BotId { get; }
    
    public TimeSpan Timeout { get; set; }
    
    public IExceptionParser ExceptionsParser { get; set; }
    
    public event AsyncEventHandler<ApiRequestEventArgs>? OnMakingApiRequest;
    
    public event AsyncEventHandler<ApiResponseEventArgs>? OnApiResponseReceived;

    public Task<TResponse> MakeRequestAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = new CancellationToken())
    {
        switch (request)
        {
            case SendMessageRequest sendMessageRequest:
                _botActions.Add(new BotActions.TextMessage(
                    Text: sendMessageRequest.Text,
                    ChatId: sendMessageRequest.ChatId.Identifier!.Value,
                    ReplyToMessageId: sendMessageRequest.ReplyToMessageId));
                break;
            
            case RestrictChatMemberRequest restrictChatMemberRequest:
                _botActions.Add(new BotActions.RestrictChatMember(
                    ChatId: restrictChatMemberRequest.ChatId.Identifier!.Value,
                    UserId: restrictChatMemberRequest.UserId,
                    CanSendMediaMessages: restrictChatMemberRequest.Permissions.CanSendMediaMessages,
                    CanSendMessages: restrictChatMemberRequest.Permissions.CanSendMessages,
                    CanSendOtherMessages: restrictChatMemberRequest.Permissions.CanSendOtherMessages,
                    CanAddWebPagePreviews: restrictChatMemberRequest.Permissions.CanAddWebPagePreviews,
                    CanChangeInfo: restrictChatMemberRequest.Permissions.CanChangeInfo,
                    CanInviteUsers: restrictChatMemberRequest.Permissions.CanInviteUsers,
                    CanPinMessages: restrictChatMemberRequest.Permissions.CanPinMessages,
                    CanSendPolls: restrictChatMemberRequest.Permissions.CanSendPolls,
                    UntilDateTime: restrictChatMemberRequest.UntilDate));
                break;

            case GetChatMemberRequest getChatMemberRequest:
            {
                var chatId = getChatMemberRequest.ChatId.Identifier!.Value;
                var chatMember = _chats[chatId].FirstOrDefault(member => member.User.Id == getChatMemberRequest.UserId);

                if (chatMember is null)
                {
                    this.ThrowRequestException("Cannot find chat member");
                }

                return (dynamic)Task.FromResult(chatMember);
            }

            case DeleteMessageRequest deleteMessageRequest:
            {
                _botActions.Add( new BotActions.DeleteMessage(
                    ChatId: deleteMessageRequest.ChatId.Identifier!.Value,
                    MessageId: deleteMessageRequest.MessageId));
                
                return (dynamic)Task.FromResult(true);
            }
        }
        
        var result = default(TResponse);
        
        return Task.FromResult(result);
    }

    public User CreateChatMember(
        User user,
        long chatId)
    {
        var member = new ChatMemberMember() { User = user };
        this.AddChatMember(chatId, member);
        return user;
    }
    
    public User CreateChatAdmin(
        long chatId,
        User user)
    {
        var member = new ChatMemberAdministrator() { User = user };
        this.AddChatMember(chatId, member);
        return user;
    }

    public void AddChatMember(
        long chatId,
        ChatMember member)
    {
        if (_chats.TryGetValue(chatId, out var members))
        {
            members.Add(member);
        }
        else
        {
            _chats[chatId] = new List<ChatMember>() { member };
        }
    }

    public Task<bool> TestApiAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task DownloadFileAsync(string filePath, Stream destination,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    private void ThrowRequestException(string errorMessage)
    {
        throw new RequestException(errorMessage);
    }

    public IEnumerable<BotActions.IBotAction> GetBotActions()
    {
        return _botActions.ToList();
    }
}