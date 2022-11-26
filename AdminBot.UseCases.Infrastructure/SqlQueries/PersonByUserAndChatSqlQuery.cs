using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace AdminBot.UseCases.Infrastructure.SqlQueries
{
    public class PersonByUserAndChatSqlQuery
    {
        public async Task<DataModels.Person> ExecuteAsync(
            IDbConnection connection,
            long userId,
            long chatId)
        {
            var reader = await connection.QueryMultipleAsync(
                sql: SqlHelp.LoadFromFile("PersonByUserAndChatSqlQuery.sql"),
                param: new
                {
                    UserId = userId,
                    ChatId = chatId
                })
                .ConfigureAwait(false);

            return await reader
                .ReadFirstOrDefaultAsync<DataModels.Person>()
                .ConfigureAwait(false);
        }
    }
}