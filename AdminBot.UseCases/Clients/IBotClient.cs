using System;
using System.Threading.Tasks;
using AdminBot.Common.Messages;

namespace AdminBot.UseCases.Clients
{
    public interface IBotClient
    {
        Task RestrictUserInChatAsync(long userId,
            long chatId,
            DateTime? untilDateTime = default);
        
        Task RemoveRestrictionAsync(
            long userId,
            long chatId);

        Task SendMessageAsync(
            IMessage message,
            long chatId);

        Task<bool> IsUserAdminAsync(
            long userId,
            long chatId);

        void SetWebHook(string webhookUrl);
        
        Task DeleteMessageAsync(long chatId, int messageId);
    }
}