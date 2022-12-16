using System;
using System.Threading;
using System.Threading.Tasks;
using AdminBot.Common;
using AdminBot.UseCases.Infrastructure.Interfaces;
using AdminBot.UseCases.Infrastructure.SqlQueries;
using AdminBot.UseCases.Repositories;
using Dapper;

namespace AdminBot.UseCases.Infrastructure.Repositories
{
    internal class PersonsRepository : IPersonsRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly PersonByUserAndChatSqlQuery _personByUserAndChatSqlQuery;
        private readonly AddPersonSqlQuery _addPersonSqlQuery;
        public PersonsRepository(IDbConnectionFactory dbConnectionFactory,
            PersonByUserAndChatSqlQuery personByUserAndChatSqlQuery,
            AddPersonSqlQuery addPersonSqlQuery)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _personByUserAndChatSqlQuery = personByUserAndChatSqlQuery;
            _addPersonSqlQuery = addPersonSqlQuery;
        }

        public async Task<Person> GetPersonAsync(
            long userId,
            long chatId,
            CancellationToken token = default)
        {
            using (var connection = _dbConnectionFactory.Create())
            {
                var result = await _personByUserAndChatSqlQuery.ExecuteAsync(
                    connection: connection,
                    userId: userId,
                    chatId: chatId);
                
                return result != null
                    ? new Person(
                        userId: result.UserId,
                        username: result.Username,
                        chatId: result.ChatId,
                        id: result.Id,
                        createdAt: result.CreatedAt,
                        warns: result.Warns,
                        updatedAt: result.UpdatedAt)
                    : null;
            }
        }

        public async Task<Person> AddPersonAsync(
            long userId,
            string username,
            long chatId,
            DateTime createdAt)
        {
            using (var connection = _dbConnectionFactory.Create())
            {
                var id = await _addPersonSqlQuery
                    .ExecuteAsync(
                        connection: connection,
                        userId: userId,
                        chatId: chatId,
                        username: username,
                        createdAt: createdAt);

                return new Person(
                    userId: userId,
                    username: username,
                    chatId: chatId,
                    id: id,
                    createdAt: createdAt,
                    warns: 0,
                    updatedAt: createdAt);
            }
        }

        public async Task<int> IncrementWarnsAsync(Person person, DateTime dateTime)
        {
            using (var connection = _dbConnectionFactory.Create())
            {
                return await connection.ExecuteScalarAsync<int>(
                        sql: $"Update Persons SET Warns=Warns+1, UpdatedAt=@UpdatedAt WHERE Id=@Id " +
                             $"SELECT Warns FROM Persons WHERE Id=@Id",
                        param: new
                        {
                            UpdatedAt = dateTime,
                            Id = person.Id,
                        })
                    .ConfigureAwait(false);
            }
        }

        public async Task ResetWarns(int personId, DateTime dateTime)
        {
            using (var connection = _dbConnectionFactory.Create())
            {
                await connection.ExecuteAsync(
                        sql: $"Update Persons SET Warns=0, UpdatedAt=@UpdatedAt WHERE Id=@Id",
                        param: new
                        {
                            UpdatedAt = dateTime,
                            Id = personId,
                        })
                    .ConfigureAwait(false);
            }
        }
    }
}