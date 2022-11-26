using System;
using System.Data;
using System.Threading.Tasks;
using AdminBot.UseCases.Infrastructure.DataModels;
using Dapper;

namespace AdminBot.UseCases.Infrastructure.SqlQueries
{
    public class AddChatSettingsSqlQuery
    {
        public async Task<ChatSettings> ExecuteAsync(IDbConnection connection,
            long telegramId,
            string agreement,
            int warnLimit,
            DateTime createdAt,
            TimeSpan banTtl)
        {
            return await connection.ExecuteScalarAsync<DataModels.ChatSettings>(
                sql: SqlHelp.LoadFromFile("AddChatSettings.sql"),
                param: new
                {
                    TelegramId = telegramId,
                    Agreement = agreement,
                    WarnsLimit = warnLimit,
                    CreatedAt = createdAt,
                    BanTtlTicks = banTtl.TotalSeconds
                }).ConfigureAwait(false);
        }
    }
}