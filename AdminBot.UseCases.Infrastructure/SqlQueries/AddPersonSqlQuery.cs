using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace AdminBot.UseCases.Infrastructure.SqlQueries
{
    public class AddPersonSqlQuery
    {
        public async Task<int> ExecuteAsync(
            IDbConnection connection,
            long userId,
            long chatId,
            string username,
            DateTime createdAt)
        {
            return await connection.ExecuteScalarAsync<int>(
                sql: SqlHelp.LoadFromFile("AddPersonSqlQuery.sql"),
                param: new
                {
                    UserId = userId,
                    ChatId = chatId,
                    Username = username,
                    Warns = 0,
                    CreatedAt = createdAt,
                    UpdatedAt = createdAt,
                })
                .ConfigureAwait(false);
        }
    }
}