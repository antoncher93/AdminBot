using System;
using System.Threading.Tasks;
using AdminBot.Common.Messages;
using AdminBot.UseCases.Clients;
using AdminBot.UseCases.Infrastructure.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AdminBot.UseCases.Infrastructure.Clients
{
    internal class BotClient : IBotClient
    {
        private readonly ITelegramBotClient _client;
        private readonly IMessageFormatter _messageFormatter;

        public BotClient(
            ITelegramBotClient client,
            IMessageFormatter messageFormatter)
        {
            _client = client;
            _messageFormatter = messageFormatter;
        }

        public async Task RestrictUserInChatAsync(long userId,
            long chatId,
            DateTime? untilDateTime)
        {
            await _client.RestrictChatMemberAsync(
                chatId: chatId,
                userId: userId,
                untilDate: untilDateTime,
                permissions: new ChatPermissions()
                {
                    CanSendMediaMessages = false,
                    CanSendMessages = false,
                    CanSendOtherMessages = false
                });
        }

        public async Task RemoveRestrictionAsync(long userId, long chatId)
        {
            var permissions = new ChatPermissions()
            {
                CanAddWebPagePreviews = true,
                CanChangeInfo = true,
                CanInviteUsers = true,
                CanPinMessages = true,
                CanSendMediaMessages = true,
                CanSendMessages = true,
                CanSendOtherMessages = true,
                CanSendPolls = true
            };

            await _client.RestrictChatMemberAsync(
                chatId: chatId,
                userId: userId,
                permissions: permissions);
        }

        public async Task SendMessageAsync(IMessage message, long chatId)
        {
            await _messageFormatter.FormatAndSend(
                message: message,
                chatId: chatId);
        }

        public async Task<bool> IsUserAdminAsync(
            long userId,
            long chatId)
        {
            var chatMember = await _client.GetChatMemberAsync(
                chatId: chatId,
                userId: userId);

            return chatMember.Status == ChatMemberStatus.Creator
                   || chatMember.Status == ChatMemberStatus.Administrator;
        }

        public void SetWebHook(string webhookUrl)
        {
            _client.SetWebhookAsync(webhookUrl)
                .Wait();
        }

        public async Task DeleteMessageAsync(
            long chatId,
            int messageId)
        {
            await _client.DeleteMessageAsync(
                chatId: chatId,
                messageId: messageId);
        }
    }
}