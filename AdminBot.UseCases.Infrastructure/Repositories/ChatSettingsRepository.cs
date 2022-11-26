using System;
using System.Threading.Tasks;
using AdminBot.Common;
using AdminBot.UseCases.Infrastructure.Interfaces;
using AdminBot.UseCases.Infrastructure.SqlQueries;
using AdminBot.UseCases.Repositories;

namespace AdminBot.UseCases.Infrastructure.Repositories
{
    internal class ChatSettingsRepository : IChatSettingsRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly AddChatSettingsSqlQuery _addChatSettingsSqlQuery;
        private readonly ChatSettingsByTelegramIdSqlQuery _chatSettingsByTelegramIdSqlQuery;
        private readonly UpdateChatSettingsSqlQuery _updateChatSettingsSqlQuery;
        private readonly TimeSpan _defaultBanTtl;
        private readonly int _defaultWarnsLimit;
        public ChatSettingsRepository(
            IDbConnectionFactory dbConnectionFactory,
            AddChatSettingsSqlQuery addChatSettingsSqlQuery,
            ChatSettingsByTelegramIdSqlQuery chatSettingsByTelegramIdSqlQuery, 
            UpdateChatSettingsSqlQuery updateChatSettingsSqlQuery, 
            TimeSpan defaultBanTtl, 
            int defaultWarnsLimit)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _defaultBanTtl = defaultBanTtl;
            _defaultWarnsLimit = defaultWarnsLimit;
            _addChatSettingsSqlQuery = addChatSettingsSqlQuery;
            _chatSettingsByTelegramIdSqlQuery = chatSettingsByTelegramIdSqlQuery;
            _updateChatSettingsSqlQuery = updateChatSettingsSqlQuery;
        }

        public async Task SaveAgreementAsync(
            long telegramId,
            string agreement,
            DateTime dateTime)
        {
            using (var connection = _dbConnectionFactory.Create())
            {
                var chatSettings = await _chatSettingsByTelegramIdSqlQuery
                    .ExecuteAsync(
                        connection: connection,
                        telegramId: telegramId)
                    .ConfigureAwait(false);

                if (chatSettings is null)
                {
                    await _addChatSettingsSqlQuery
                        .ExecuteAsync(
                            connection: connection,
                            telegramId: telegramId,
                            agreement: agreement,
                            warnLimit: _defaultWarnsLimit,
                            createdAt: dateTime,
                            banTtl: _defaultBanTtl)
                        .ConfigureAwait(false);
                }
                else
                {
                    await _updateChatSettingsSqlQuery
                        .ExecuteAsync(
                            connection: connection,
                            id: chatSettings.Id,
                            agreement: agreement,
                            banTtl: TimeSpan.FromSeconds(chatSettings.BanTtlTicks))
                        .ConfigureAwait(false);
                }
            }
        }

        public async Task<ChatSettings> FindAsync(long chatId)
        {
            using (var connection = _dbConnectionFactory.Create())
            {
                var model = await _chatSettingsByTelegramIdSqlQuery
                    .ExecuteAsync(
                        connection: connection,
                        telegramId: chatId);

                return model != null
                    ? new ChatSettings(
                        telegramId: model.TelegramId,
                        agreement: model.Agreement,
                        warnsLimit: model.WarnsLimit,
                        banTtl: TimeSpan.FromSeconds(model.BanTtlTicks),
                        createdAt: model.CreatedAt)
                    : null;
            }
        }
    }
}