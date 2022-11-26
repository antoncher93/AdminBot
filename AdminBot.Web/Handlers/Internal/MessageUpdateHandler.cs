using AdminBot.Common.Commands;
using AdminBot.Common.Queries;
using AdminBot.Web.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AdminBot.Web.Handlers.Internal;

public class MessageUpdateHandler : IMessageUpdateHandler
{
    private readonly IBotCommandMessageHandler _botCommandMessageHandler;
    private readonly RegisterPersonQuery.IHandler _registerPersonCommandHandler;
    private readonly WelcomePersonCommand.IHandler _welcomePersonCommandHandler;
    private readonly ChatSettingsQuery.IHandler _chatSettingsQueryHandler;

    public MessageUpdateHandler(
        IBotCommandMessageHandler botCommandMessageHandler,
        WelcomePersonCommand.IHandler welcomePersonCommandHandler,
        RegisterPersonQuery.IHandler registerPersonCommandHandler,
        ChatSettingsQuery.IHandler chatSettingsQueryHandler)
    {
        _botCommandMessageHandler = botCommandMessageHandler;
        _welcomePersonCommandHandler = welcomePersonCommandHandler;
        _registerPersonCommandHandler = registerPersonCommandHandler;
        _chatSettingsQueryHandler = chatSettingsQueryHandler;
    }

    public async Task HandleAsync(
        Message message,
        DateTime receivedAt)
    {
        await message.Match(
            onTextMessage: async message => await OnTextMessageAsync(message, receivedAt),
            onChatMembersAdded: async message => await OnChatMembersAddedAsync(message, receivedAt),
            onDefault: async message => await OnDefaultAsync(message));
    }

    private Task OnDefaultAsync(
        Message message)
    {
        return Task.CompletedTask;
    }

    private async Task OnChatMembersAddedAsync(
        Message message,
        DateTime dateTime)
    {
        var chatSettings = await _chatSettingsQueryHandler
            .HandleAsync(
                query: new ChatSettingsQuery(
                    chatId: message.Chat.Id));
        
        if (message.NewChatMembers != null
            && chatSettings != null)
        {
            foreach (var chatMember in message.NewChatMembers)
            {
                var person = await _registerPersonCommandHandler
                    .HandleAsync(
                        query: new RegisterPersonQuery(
                            userId: chatMember.Id,
                            chatId: message.Chat.Id,
                            userName: chatMember.Username,
                            dateTime: dateTime));

                await _welcomePersonCommandHandler
                    .HandleAsync(
                        command: new WelcomePersonCommand(
                            person: person,
                            agreement: chatSettings.Agreement));
            }   
        }
    }

    private async Task OnTextMessageAsync(
        Message message,
        DateTime receivedAt)
    {
        var botCommandEntity = message?.Entities?.FirstOrDefault(entity => entity.Type == MessageEntityType.BotCommand);
        
        if (botCommandEntity != null)
        {
            var command = message?.Text?.Substring(
                startIndex: botCommandEntity.Offset,
                length: botCommandEntity.Length);
            
            await _botCommandMessageHandler.HandleAsync(
                    command: command,
                    message: message,
                    receivedAt: receivedAt)
                .ConfigureAwait(false);
        }
    }
}