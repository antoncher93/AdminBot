using System;
using System.Threading.Tasks;
using AdminBot.Common;
using AdminBot.UseCases.Infrastructure.Interfaces;
using AdminBot.UseCases.Repositories;
using Dapper;

namespace AdminBot.UseCases.Infrastructure.Repositories
{
    public class BanRepository : IBanRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public BanRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<Ban> AddAsync(Person person,
            DateTime requestedAt,
            DateTime expireAt)
        {
            using (var connection = _dbConnectionFactory.Create())
            {
                await connection.ExecuteAsync(
                    sql: "INSERT INTO Bans (PersonId, CreatedAt, ExpireAt) "
                         + $"Values (@PersonId, @CreatedAt, @ExpireAt)",
                    param: new
                    {
                        PersonId = person.Id,
                        CreatedAt = requestedAt,
                        ExpireAt = expireAt,
                    })
                    .ConfigureAwait(false);
            }

            return new Ban(
                person: person,
                expireAt: expireAt,
                createdAt: requestedAt);
        }

        public async Task<Ban> FindForPerson(Person person)
        {
            using (var connection = _dbConnectionFactory.Create())
            {
                var model = await connection.QueryFirstOrDefaultAsync<DataModels.Ban>(
                        sql: $"SELECT * FROM Bans WHERE PersonId={person.Id}")
                    .ConfigureAwait(false);

                return model != null
                       ? new Ban(
                           person: person,
                           expireAt: model.ExpireAt,
                           createdAt: model.CreatedAt)
                       : null;
            }
        }
    }
}