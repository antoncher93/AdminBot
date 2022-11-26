using AdminBot.UseCases.Infrastructure.Internal;
using AdminBot.UseCases.Infrastructure.Repositories;
using AdminBot.UseCases.Infrastructure.SqlQueries;
using AdminBot.UseCases.Repositories;

namespace AdminBot.UseCases.Infrastructure
{
    public static class PersonsRepositoryFactory
    {
        public static IPersonsRepository Create(
            string sqlConnectionString)
        {
            var dbConnectionFactory = new SqlConnectionFactory(
                connectionString: sqlConnectionString);
            return new PersonsRepository
            (
                dbConnectionFactory: dbConnectionFactory,
                personByUserAndChatSqlQuery: new PersonByUserAndChatSqlQuery(),
                addPersonSqlQuery: new AddPersonSqlQuery());
        }
    }
}