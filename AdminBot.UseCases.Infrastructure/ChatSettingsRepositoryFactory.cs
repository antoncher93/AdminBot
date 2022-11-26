using System;
using AdminBot.UseCases.Infrastructure.Internal;
using AdminBot.UseCases.Infrastructure.Repositories;
using AdminBot.UseCases.Infrastructure.SqlQueries;
using AdminBot.UseCases.Repositories;

namespace AdminBot.UseCases.Infrastructure
{
    public static class ChatSettingsRepositoryFactory
    {
        public static IChatSettingsRepository Create(
            string sqlConnectionString,
            TimeSpan defaultBanTtl,
            int defaultWarnsLimit)
        {
            var dbConnectionFactory = new SqlConnectionFactory(sqlConnectionString);
            return new ChatSettingsRepository(
                dbConnectionFactory: dbConnectionFactory,
                addChatSettingsSqlQuery: new AddChatSettingsSqlQuery(),
                chatSettingsByTelegramIdSqlQuery: new ChatSettingsByTelegramIdSqlQuery(),
                updateChatSettingsSqlQuery: new UpdateChatSettingsSqlQuery(),
                defaultBanTtl: defaultBanTtl,
                defaultWarnsLimit: defaultWarnsLimit);
        }
    }
}