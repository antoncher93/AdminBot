using System;
using System.Threading.Tasks;
using AdminBot.Common;

namespace AdminBot.UseCases.Repositories
{
    public interface IChatSettingsRepository
    {
        Task SaveAgreementAsync(
            long telegramId,
            string agreement,
            TimeSpan banTtl,
            int warnsLimit,
            DateTime dateTime);

        Task<ChatSettings> FindAsync(
            long chatId);
    }
}