using System.Data;
using System.Data.SqlClient;
using AdminBot.UseCases.Infrastructure.Interfaces;

namespace AdminBot.UseCases.Infrastructure.Internal
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public IDbConnection Create()
        {
            var dbConnection = new SqlConnection(_connectionString);
            dbConnection.Open();
            return dbConnection;
        }
    }
}