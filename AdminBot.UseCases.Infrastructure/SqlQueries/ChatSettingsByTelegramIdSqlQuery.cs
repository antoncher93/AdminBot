using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace AdminBot.UseCases.Infrastructure.SqlQueries
{
    public class ChatSettingsByTelegramIdSqlQuery
    {
        public async Task<DataModels.ChatSettings> ExecuteAsync(
            IDbConnection connection,
            long telegramId)
        {
            return await connection
                .QueryFirstOrDefaultAsync<DataModels.ChatSettings>(
                    sql: $"SELECT * FROM ChatSettings WHERE TelegramId={telegramId}")
                .ConfigureAwait(false);
        }
    }
}