using AdminBot.UseCases.Infrastructure;
using AdminBot.UseCases.Infrastructure.Internal;
using AdminBot.UseCases.Infrastructure.Repositories;
using AdminBot.UseCases.Infrastructure.SqlQueries;

namespace AdminBot.Web.Tests.RepositoriesTests;

public static class SutFactory
{
    private const string ConnectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=Pa23@!Ze7&;";
    public static Sut Create()
    {
        var connectionFactory = new SqlConnectionFactory(
            connectionString: ConnectionString);

        var personsRepository = PersonsRepositoryFactory.Create(ConnectionString);

        var chatAgreementRepository = ChatSettingsRepositoryFactory.Create(
            sqlConnectionString: ConnectionString);

        var banRepository = new BanRepository(
            dbConnectionFactory: connectionFactory);

        return new Sut(
            connectionString: ConnectionString,
            personsRepository: personsRepository,
            chatSettingsRepository: chatAgreementRepository,
            banRepository: banRepository);
    }
}