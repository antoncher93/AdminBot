using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace AdminBot.UseCases.Infrastructure.SqlQueries
{
    public class UpdateChatSettingsSqlQuery
    {
        public async Task ExecuteAsync(
            IDbConnection connection,
            int id,
            string agreement,
            TimeSpan banTtl)
        {
            await connection.ExecuteAsync(
                sql: SqlHelp.LoadFromFile("UpdateChatSettings.sql"),
                param: new
                {
                    Id = id,
                    Agreement = agreement,
                    BanTtlTicks = banTtl.TotalSeconds,
                })
                .ConfigureAwait(false);
        }
    }
}